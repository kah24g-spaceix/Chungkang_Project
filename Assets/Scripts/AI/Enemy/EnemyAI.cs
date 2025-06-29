using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;
using System.Collections;

public class EnemyAI : BehaviorTreeBase
{
    [Header("Configuration")]
    public EnemyConfig config;
    
    [Header("Patrol Points")]
    public Transform[] patrolPoints;
    
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
    
    // 동적 시야 시스템
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

        // Validate components
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component missing on " + gameObject.name);
        }

        if (animator == null)
        {
            Debug.LogError("Animator component missing on " + gameObject.name);
        }
        else
        {
            // Initialize animator parameters
            InitializeAnimatorParameters();
        }

        // Initialize sight system
        InitializeSightSystem();

        // Initialize blackboard
        blackboard = new Blackboard();
        InitializeBlackboard();

        // Setup AI
        if (config != null)
        {
            agent.speed = config.walkSpeed;
            currentHealth = config.maxHealth;
        }

        isInitialized = true;
        base.Start();
        
        CreateHealthBar();
    }
    
    private void CreateHealthBar()
{
    // Use HealthBarManager for automatic assignment
    if (HealthBarManager.Instance != null)
    {
        healthBarInstance = HealthBarManager.Instance.RequestHealthBar(transform, this);
        
        if (healthBarInstance != null)
        {
            Debug.Log($"Health bar automatically assigned to {gameObject.name}");
        }
    }
    else
    {
        // Fallback to old method if HealthBarManager not found
        if (healthBarPrefab != null)
        {
            GameObject healthBarGO = Instantiate(healthBarPrefab);
            healthBarInstance = healthBarGO.GetComponent<EnemyHealthBar>();
            
            if (healthBarInstance != null)
            {
                healthBarInstance.Initialize(transform, this);
            }
        }
        else
        {
            Debug.LogWarning($"No HealthBarManager found and no healthBarPrefab assigned for {gameObject.name}");
        }
    }
}

    private void InitializeAnimatorParameters()
    {
        if (animator == null) return;

        // Initialize all animation parameters to default values
        SetAnimatorFloat("Speed", 0f);
        SetAnimatorFloat("HealthPercentage", 1f);
        SetAnimatorBool("IsAttacking", false);
        SetAnimatorBool("IsPatrolling", false);
        SetAnimatorBool("IsFleeing", false);
        SetAnimatorBool("IsInvestigating", false);

        Debug.Log("Animator parameters initialized for " + gameObject.name);
    }
    
    private void InitializeSightSystem()
    {
        if (config != null)
        {
            currentSightRange = config.sightRange;
            currentSightAngle = config.sightAngle;
            targetSightRange = config.sightRange;
            targetSightAngle = config.sightAngle;
        }
    }
    
    protected override void Update()
    {
        if (!isInitialized) return;
        
        UpdateSightSystem();
        UpdateBlackboard();
        UpdateAnimations();
        base.Update();
    }
    
    private void UpdateAnimations()
    {
        if (animator == null || agent == null) return;
        
        // Smooth speed animation
        float targetSpeed = agent.velocity.magnitude;
        currentAnimationSpeed = Mathf.Lerp(currentAnimationSpeed, targetSpeed, 
            speedSmoothingRate * Time.deltaTime);
        
        SetAnimatorFloat("Speed", currentAnimationSpeed * animationSpeedMultiplier);
        
        // Update health percentage
        if (config != null)
        {
            float healthPercent = currentHealth / config.maxHealth;
            SetAnimatorFloat("HealthPercentage", healthPercent);
        }
    }
    
    // Safe animator parameter setters with null checks
    private void SetAnimatorFloat(string parameterName, float value)
    {
        if (animator != null && HasAnimatorParameter(parameterName))
        {
            animator.SetFloat(parameterName, value);
        }
    }
    
    private void SetAnimatorBool(string parameterName, bool value)
    {
        if (animator != null && HasAnimatorParameter(parameterName))
        {
            animator.SetBool(parameterName, value);
        }
    }
    
    private void SetAnimatorTrigger(string parameterName)
    {
        if (animator != null && HasAnimatorParameter(parameterName))
        {
            animator.SetTrigger(parameterName);
        }
    }
    
    private bool HasAnimatorParameter(string parameterName)
    {
        if (animator == null || animator.runtimeAnimatorController == null) return false;
        
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName)
                return true;
        }
        return false;
    }
    
    private void UpdateSightSystem()
    {
        if (config == null) return;
        
        bool playerVisible = blackboard.GetValue<bool>("isPlayerVisible");
        
        // 시야 상태 업데이트
        UpdateSightState(playerVisible);
        
        // 목표 시야 값 설정
        SetTargetSightValues();
        
        // 부드러운 시야 전환
        SmoothSightTransition();
        
        // 블랙보드에 현재 시야 값 업데이트
        blackboard.SetValue("currentSightRange", currentSightRange);
        blackboard.SetValue("currentSightAngle", currentSightAngle);
        blackboard.SetValue("sightState", currentSightState);
    }
    
    private void UpdateSightState(bool playerVisible)
    {
        if (playerVisible)
        {
            // 플레이어 발견
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
            // 플레이어를 놓쳤을 때
            float timeSinceLastSeen = Time.time - lastPlayerSeenTime;
            
            if (currentSightState == SightState.Alerted && timeSinceLastSeen > config.alertedStateDuration)
            {
                currentSightState = SightState.Normal;
                hasEverSeenPlayer = false;
                Debug.Log("Enemy sight state changed to: Normal");
            }
        }
    }
    
    private void SetTargetSightValues()
    {
        switch (currentSightState)
        {
            case SightState.Normal:
                targetSightRange = config.sightRange;
                targetSightAngle = config.sightAngle;
                break;
                
            case SightState.Alerted:
            case SightState.Investigating:
                targetSightRange = config.alertedSightRange;
                targetSightAngle = config.alertedSightAngle;
                break;
        }
    }
    
    private void SmoothSightTransition()
    {
        float transitionSpeed = config.sightTransitionSpeed * Time.deltaTime;
        
        currentSightRange = Mathf.Lerp(currentSightRange, targetSightRange, transitionSpeed);
        currentSightAngle = Mathf.Lerp(currentSightAngle, targetSightAngle, transitionSpeed);
    }
    
    private void InitializeBlackboard()
    {
        // Find player with Player tag
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            player = playerGO.transform;
            blackboard.SetValue("player", player);
            
            // 플레이어의 AndroidPlayerStats 컴포넌트도 저장
            var androidStats = playerGO.GetComponent<AndroidPlayerStats>();
            if (androidStats != null)
            {
                blackboard.SetValue("androidStats", androidStats);
            }
        }
        
        // Initialize other values
        blackboard.SetValue("agent", agent);
        blackboard.SetValue("config", config);
        blackboard.SetValue("patrolPoints", patrolPoints);
        blackboard.SetValue("currentPatrolIndex", 0);
        blackboard.SetValue("lastAttackTime", 0f);
        blackboard.SetValue("isPlayerVisible", false);
        blackboard.SetValue("distanceToPlayer", float.MaxValue);
        blackboard.SetValue("transform", transform);
        blackboard.SetValue("animator", animator);
        blackboard.SetValue("lastKnownPlayerPosition", Vector3.zero);
        blackboard.SetValue("currentSightRange", currentSightRange);
        blackboard.SetValue("currentSightAngle", currentSightAngle);
        blackboard.SetValue("sightState", currentSightState);
    }
    
    private void UpdateBlackboard()
    {
        if (player != null && config != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            bool canSeePlayer = CanSeePlayer();
            
            blackboard.SetValue("distanceToPlayer", distance);
            blackboard.SetValue("isPlayerVisible", canSeePlayer);
            blackboard.SetValue("healthPercentage", currentHealth / config.maxHealth);
            blackboard.SetValue("currentHealth", currentHealth);
            
            // 플레이어가 보이면 마지막 위치 업데이트
            if (canSeePlayer)
            {
                blackboard.SetValue("lastKnownPlayerPosition", player.position);
            }
        }
    }
    
    private bool CanSeePlayer()
    {
        if (player == null || config == null) return false;
        
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        
        // 현재 동적 시야각 사용
        if (angle > currentSightAngle * 0.5f) return false;
        
        // 현재 동적 시야 거리 사용
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > currentSightRange) return false;
        
        // Check line of sight
        Ray ray = new Ray(transform.position + Vector3.up, directionToPlayer);
        if (Physics.Raycast(ray, out RaycastHit hit, currentSightRange, config.obstacleLayer))
        {
            return hit.collider.CompareTag("Player");
        }
        
        return true;
    }
    
    protected override BehaviorNode SetupTree()
    {
        if (config == null)
        {
            Debug.LogError("EnemyConfig is not assigned!");
            return new PatrolTask(blackboard);
        }
        
        Selector root = new Selector();
        
        // Health check -> Flee
        Sequence fleeSequence = new Sequence();
        fleeSequence.AddChild(new CheckHealthLow(blackboard));
        fleeSequence.AddChild(new FleeTask(blackboard));
        
        // Combat sequence
        Sequence combatSequence = new Sequence();
        combatSequence.AddChild(new CheckPlayerVisible(blackboard));
        combatSequence.AddChild(new CheckPlayerInRange(blackboard));
        combatSequence.AddChild(new AttackTask(blackboard));
        
        // Chase sequence
        Sequence chaseSequence = new Sequence();
        chaseSequence.AddChild(new CheckPlayerVisible(blackboard));
        chaseSequence.AddChild(new ChaseTask(blackboard));
        
        // Investigate sequence
        Sequence investigateSequence = new Sequence();
        investigateSequence.AddChild(new CheckLastKnownPosition(blackboard));
        investigateSequence.AddChild(new InvestigateTask(blackboard));
        
        // Default patrol
        PatrolTask patrolTask = new PatrolTask(blackboard);
        
        // Build tree
        root.AddChild(fleeSequence);
        root.AddChild(combatSequence);
        root.AddChild(chaseSequence);
        root.AddChild(investigateSequence);
        root.AddChild(patrolTask);
        
        return root;
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        // Update health bar
        if (healthBarInstance != null && config != null)
        {
            float healthPercentage = currentHealth / config.maxHealth;
            healthBarInstance.UpdateHealthBar(healthPercentage);
        }
        
        // Hit reaction animation
        if (animator != null && currentHealth > 0)
        {
            SetAnimatorTrigger("Hit");
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    
    private void Die()
    {
        enabled = false;
        if (agent != null) agent.enabled = false;
        
        // Notify health bar about death
        if (healthBarInstance != null)
        {
            healthBarInstance.OnEnemyDestroyed();
        }
        
        // Trigger death animation
        SetAnimatorTrigger("Die");
        
        Invoke(nameof(DisableEnemy), 3f);
    }

    
    private void DisableEnemy()
    {
        gameObject.SetActive(false);
    }
    
    // Public methods for BT nodes to control animations
    public void SetPatrolAnimation(bool isPatrolling)
    {
        SetAnimatorBool("IsPatrolling", isPatrolling);
    }
    
    public void SetFleeAnimation(bool isFleeing)
    {
        SetAnimatorBool("IsFleeing", isFleeing);
    }
    
    public void SetInvestigateAnimation(bool isInvestigating)
    {
        SetAnimatorBool("IsInvestigating", isInvestigating);
    }
    
    public void TriggerAttackAnimation()
    {
        SetAnimatorTrigger("Attack");
        SetAnimatorBool("IsAttacking", true);
    }
    
    public void StopAttackAnimation()
    {
        SetAnimatorBool("IsAttacking", false);
    }
    
    void OnDrawGizmosSelected()
    {
        if (config == null) return;
        
        // 현재 시야 거리 표시 (동적)
        Gizmos.color = GetSightGizmoColor();
        Gizmos.DrawWireSphere(transform.position, currentSightRange);
        
        // 공격 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, config.attackRange);
        
        // 현재 시야각 표시 (동적)
        Vector3 leftBoundary = Quaternion.Euler(0, -currentSightAngle * 0.5f, 0) * transform.forward * currentSightRange;
        Vector3 rightBoundary = Quaternion.Euler(0, currentSightAngle * 0.5f, 0) * transform.forward * currentSightRange;
        
        Gizmos.color = GetSightGizmoColor();
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        
        // 시야 상태 텍스트 표시
        if (Application.isPlaying)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 3, 
                $"Sight State: {currentSightState}\n" +
                $"Range: {currentSightRange:F1}/{targetSightRange:F1}\n" +
                $"Angle: {currentSightAngle:F1}°/{targetSightAngle:F1}°");
        }
    }
    
    private Color GetSightGizmoColor()
    {
        switch (currentSightState)
        {
            case SightState.Normal:
                return Color.yellow;
            case SightState.Alerted:
                return new Color(1f, 0.5f, 0f); // Orange color
            case SightState.Investigating:
                return Color.red;
            default:
                return Color.white;
        }
    }
}
