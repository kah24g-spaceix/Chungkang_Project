using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;
using System.Linq;

public class EnemyAI : BehaviorTreeBase
{
    [Header("Configuration")]
    public EnemyConfig config;
    public BlackboardConfig blackboardConfig;

    [Header("Animation Settings")]
    [SerializeField] private float animationSpeedMultiplier = 1f;
    [SerializeField] private float speedSmoothingRate = 5f;

    [Header("Health Bar")]
    [SerializeField] private GameObject healthBarPrefab;
    private EnemyHealthBar healthBarInstance;

    // Components
    private NavMeshAgent agent;
    private Animator animator;

    // Blackboard data
    private Blackboard blackboard;
    private Transform player;
    private float currentHealth;

    // Animation state tracking
    private float currentAnimationSpeed = 0f;
    private bool isInitialized = false;

    // Death flag
    private bool isDead = false;

    // Animator parameter hashes
    private int speedHash;
    private int healthPercentageHash;
    private int isAttackingHash;
    private int isFleeingHash;
    private int isInvestigatingHash;
    private int hitTriggerHash;
    private int dieTriggerHash;

    // Dynamic sight system
    private SightState currentSightState = SightState.Normal;
    private float currentSightRange;
    private float currentSightAngle;
    private float targetSightRange;
    private float targetSightAngle;
    private float lastPlayerSeenTime;
    private bool hasEverSeenPlayer = false;

    protected override void Start()
    {
        // Initialize components
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent == null)
            Debug.LogError("NavMeshAgent component missing on " + gameObject.name);
        if (animator == null)
            Debug.LogError("Animator component missing on " + gameObject.name);

        // Cache animator hashes and initialize parameters
        CacheAnimatorHashes();
        InitializeAnimatorParameters();

        // Initialize sight system
        InitializeSightSystem();

        // Initialize blackboard
        blackboard = new Blackboard();
        InitializeBlackboard();

        // Setup AI parameters
        if (config != null)
        {
            agent.speed = config.walkSpeed;
            currentHealth = config.maxHealth;
        }

        isInitialized = true;
        base.Start();

        // Create health bar
        CreateHealthBar();
    }

    private void InitializeSightSystem()
    {
        if (config == null) return;

        currentSightRange = config.sightRange;
        currentSightAngle = config.sightAngle;
        targetSightRange = config.sightRange;
        targetSightAngle = config.sightAngle;
    }

    private void CacheAnimatorHashes()
    {
        speedHash = Animator.StringToHash("Speed");
        healthPercentageHash = Animator.StringToHash("HealthPercentage");
        isAttackingHash = Animator.StringToHash("IsAttacking");
        isFleeingHash = Animator.StringToHash("IsFleeing");
        isInvestigatingHash = Animator.StringToHash("IsInvestigating");
        hitTriggerHash = Animator.StringToHash("Hit");
        dieTriggerHash = Animator.StringToHash("Die");
    }

    private void InitializeAnimatorParameters()
    {
        if (animator == null) return;

        animator.SetFloat(speedHash, 0f);
        animator.SetFloat(healthPercentageHash, 1f);
        animator.SetBool(isAttackingHash, false);
        animator.SetBool(isFleeingHash, false);
        animator.SetBool(isInvestigatingHash, false);

        Debug.Log("Animator parameters initialized for " + gameObject.name);
    }

    private void CreateHealthBar()
    {
        if (HealthBarManager.Instance != null)
        {
            healthBarInstance = HealthBarManager.Instance.RequestHealthBar(transform, this);
        }
        else if (healthBarPrefab != null)
        {
            GameObject go = Instantiate(healthBarPrefab);
            healthBarInstance = go.GetComponent<EnemyHealthBar>();
            healthBarInstance?.Initialize(transform, this);
        }
        else
        {
            Debug.LogWarning($"No HealthBarManager and no prefab for {gameObject.name}");
        }
    }

    protected override void Update()
    {
        if (!isInitialized || isDead) return;

        UpdateSightSystem();
        UpdateBlackboard();
        UpdateAnimations();
        base.Update();
    }

    private void UpdateAnimations()
    {
        if (animator == null || agent == null) return;

        float targetSpeed = agent.velocity.magnitude;
        currentAnimationSpeed = Mathf.Lerp(currentAnimationSpeed, targetSpeed,
            speedSmoothingRate * Time.deltaTime);

        animator.SetFloat(speedHash, currentAnimationSpeed * animationSpeedMultiplier);

        if (config != null)
        {
            float healthPercent = currentHealth / config.maxHealth;
            animator.SetFloat(healthPercentageHash, healthPercent);
        }
    }

    private void UpdateSightSystem()
    {
        if (config == null) return;

        bool playerVisible = blackboard.GetValue<bool>(BlackboardKey.IsPlayerVisible);
        UpdateSightState(playerVisible);
        SetTargetSightValues();
        SmoothSightTransition();

        blackboard.SetValue(BlackboardKey.CurrentSightRange, currentSightRange);
        blackboard.SetValue(BlackboardKey.CurrentSightAngle, currentSightAngle);
        blackboard.SetValue(BlackboardKey.SightState, currentSightState);
    }

    private void UpdateSightState(bool playerVisible)
    {
        if (playerVisible)
        {
            lastPlayerSeenTime = Time.time;
            hasEverSeenPlayer = true;
            if (currentSightState == SightState.Normal)
            {
                currentSightState = SightState.Alerted;
                Debug.Log("Enemy sight state changed to: Alerted");
            }
        }
        else if (hasEverSeenPlayer)
        {
            float elapsed = Time.time - lastPlayerSeenTime;
            if (currentSightState == SightState.Alerted && elapsed > config.alertedStateDuration)
            {
                currentSightState = SightState.Normal;
                hasEverSeenPlayer = false;
                Debug.Log("Enemy sight state changed to: Normal");
            }
        }
    }

    private void SetTargetSightValues()
    {
        if (currentSightState == SightState.Normal)
        {
            targetSightRange = config.sightRange;
            targetSightAngle = config.sightAngle;
        }
        else
        {
            targetSightRange = config.alertedSightRange;
            targetSightAngle = config.alertedSightAngle;
        }
    }

    private void SmoothSightTransition()
    {
        float t = config.sightTransitionSpeed * Time.deltaTime;
        currentSightRange = Mathf.Lerp(currentSightRange, targetSightRange, t);
        currentSightAngle = Mathf.Lerp(currentSightAngle, targetSightAngle, t);
    }

    private void InitializeBlackboard()
    {
        // 먼저 기본값들을 BlackboardConfig에서 설정
        if (blackboardConfig != null)
        {
            blackboardConfig.InitializeDefaultValues(blackboard);
        }
        
        // 런타임에서 설정해야 하는 값들
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            player = playerGO.transform;
            blackboard.SetValue(BlackboardKey.Player, player);
            var stats = playerGO.GetComponent<AndroidPlayerStats>();
            if (stats != null) blackboard.SetValue(BlackboardKey.AndroidStats, stats);
        }
        
        blackboard.SetValue(BlackboardKey.Agent, agent);
        blackboard.SetValue(BlackboardKey.Config, config);
        blackboard.SetValue(BlackboardKey.Transform, transform);
        blackboard.SetValue(BlackboardKey.Animator, animator);
        blackboard.SetValue(BlackboardKey.SpawnPosition, transform.position);
        blackboard.SetValue(BlackboardKey.CurrentSightRange, currentSightRange);
        blackboard.SetValue(BlackboardKey.CurrentSightAngle, currentSightAngle);
        blackboard.SetValue(BlackboardKey.SightState, currentSightState);
        
        blackboard.SetValue(BlackboardKey.PatrolPoints, GetPatrolPoints());
        
        // BlackboardConfig로 검증
        if (blackboardConfig != null)
        {
            blackboardConfig.ValidateBlackboard(blackboard);
        }
    }

    private void UpdateBlackboard()
    {
        if (player == null || config == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        bool canSee = CanSeePlayer();

        blackboard.SetValue(BlackboardKey.DistanceToPlayer, dist);
        blackboard.SetValue(BlackboardKey.IsPlayerVisible, canSee);
        blackboard.SetValue(BlackboardKey.HealthPercentage, currentHealth / config.maxHealth);
        blackboard.SetValue(BlackboardKey.CurrentHealth, currentHealth);

        if (canSee)
            blackboard.SetValue(BlackboardKey.LastKnownPlayerPosition, player.position);
    }

    private bool CanSeePlayer()
    {
        if (player == null || config == null) return false;

        Vector3 dir = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > currentSightAngle * 0.5f) return false;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > currentSightRange) return false;

        Ray ray = new Ray(transform.position + Vector3.up, dir);
        RaycastHit[] hits = Physics.RaycastAll(ray, currentSightRange);
        foreach (var hit in hits)
        {
            // Check for layer instead of tag
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                return false;
            if (hit.collider.CompareTag("Player"))
                return true;
        }
        return false;
    }

    protected override BehaviorNode SetupTree()
    {
        if (config == null)
        {
            Debug.LogError("EnemyConfig is not assigned!");
            return new Sequence(); // 기본 동작 없음
        }

        var root = new Selector();

        var fleeSeq = new Sequence();
        fleeSeq.AddChild(new CheckHealthLow(blackboard));
        fleeSeq.AddChild(new FleeTask(blackboard));

        var combatSeq = new Sequence();
        combatSeq.AddChild(new CheckPlayerVisible(blackboard));
        combatSeq.AddChild(new CheckPlayerInRange(blackboard));
        combatSeq.AddChild(new AttackTask(blackboard));

        var chaseSeq = new Sequence();
        chaseSeq.AddChild(new CheckPlayerVisible(blackboard));
        chaseSeq.AddChild(new ChaseTask(blackboard));

        var invSeq = new Sequence();
        invSeq.AddChild(new CheckLastKnownPosition(blackboard));
        invSeq.AddChild(new InvestigateTask(blackboard));

        var patrolTask = new PatrolTask(blackboard);

        root.AddChild(fleeSeq);
        root.AddChild(combatSeq);
        root.AddChild(chaseSeq);
        root.AddChild(invSeq);
        root.AddChild(patrolTask);

        return root;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        if (healthBarInstance != null && config != null)
        {
            float pct = currentHealth / config.maxHealth;
            healthBarInstance.UpdateHealthBar(pct);
        }

        if (animator != null && currentHealth > 0)
        {
            animator.SetTrigger(hitTriggerHash);
        }

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        isDead = true;
        enabled = false;
        if (agent != null) agent.enabled = false;

        if (healthBarInstance != null)
            healthBarInstance.OnEnemyDestroyed();

        animator?.SetTrigger(dieTriggerHash);
        Invoke(nameof(DisableEnemy), 3f);
    }

    private void DisableEnemy() => gameObject.SetActive(false);

    public void SetFleeAnimation(bool val) => animator?.SetBool(isFleeingHash, val);
    public void SetInvestigateAnimation(bool val) => animator?.SetBool(isInvestigatingHash, val);
    public void TriggerAttackAnimation() => animator?.SetTrigger(isAttackingHash);
    public void StopAttackAnimation() => animator?.SetBool(isAttackingHash, false);

    void OnDrawGizmosSelected()
    {
        if (config == null) return;

        Gizmos.color = GetSightGizmoColor();
        Gizmos.DrawWireSphere(transform.position, currentSightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, config.attackRange);

        Vector3 left = Quaternion.Euler(0, -currentSightAngle * 0.5f, 0) * transform.forward * currentSightRange;
        Vector3 right = Quaternion.Euler(0, currentSightAngle * 0.5f, 0) * transform.forward * currentSightRange;
        Gizmos.color = GetSightGizmoColor();
        Gizmos.DrawLine(transform.position, transform.position + left);
        Gizmos.DrawLine(transform.position, transform.position + right);

        if (Application.isPlaying)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 3,
                $"Sight: {currentSightState}\nRange: {currentSightRange:F1}/{targetSightRange:F1}\nAngle: {currentSightAngle:F1}\u00B0/{targetSightAngle:F1}\u00B0");
        }
    }

    private Color GetSightGizmoColor()
    {
        switch (currentSightState)
        {
            case SightState.Normal: return Color.yellow;
            case SightState.Alerted: return new Color(1f, 0.5f, 0f);
            case SightState.Investigating: return Color.red;
            default: return Color.white;
        }
    }
    private Transform[] GetPatrolPoints()
    {
        // 자식 오브젝트 중 "PatrolPoint"라는 이름을 가진 것들을 찾거나
        // Inspector에서 설정할 수 있도록 public 배열을 추가할 수 있습니다.
        return GetComponentsInChildren<Transform>().Where(t => t.name.Contains("PatrolPoint")).ToArray();
    }
}
