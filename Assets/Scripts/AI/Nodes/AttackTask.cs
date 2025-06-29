using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class AttackTask : BehaviorNode
{
    private Blackboard blackboard;
    private float attackTimer = 0f;
    private bool isAttacking = false;
    
    public AttackTask(Blackboard blackboard)
    {
        this.blackboard = blackboard;
    }
    
    public override NodeState Evaluate()
    {
        NavMeshAgent agent = blackboard.GetValue<NavMeshAgent>("agent");
        EnemyConfig config = blackboard.GetValue<EnemyConfig>("config");
        Animator animator = blackboard.GetValue<Animator>("animator");
        Transform transform = blackboard.GetValue<Transform>("transform");
        Transform player = blackboard.GetValue<Transform>("player");
        
        // Stop moving when attacking
        agent.SetDestination(transform.position);
        
        // Face the player
        if (player != null)
        {
            Vector3 lookDirection = (player.position - transform.position).normalized;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, 
                    Quaternion.LookRotation(lookDirection), 
                    config.rotationSpeed * Time.deltaTime);
            }
        }
        
        if (!isAttacking)
        {
            float lastAttackTime = blackboard.GetValue<float>("lastAttackTime");
            
            if (Time.time - lastAttackTime >= config.attackCooldown)
            {
                // Start attack
                isAttacking = true;
                attackTimer = 0f;
                blackboard.SetValue("lastAttackTime", Time.time);
                
                // Trigger attack animation
                if (animator != null)
                {
                    animator.SetTrigger("Attack");
                }
                
                // Deal damage to player using existing HP system
                if (player != null)
                {
                    // BasePlayerStats를 사용하여 데미지 적용
                    var playerStats = player.GetComponent<BasePlayerStats>();
                    if (playerStats != null)
                    {
                        // EnemyConfig의 attackDamage는 float이므로 int로 변환
                        int damage = Mathf.RoundToInt(config.attackDamage);
                        playerStats.TakeDamage(damage);
                        
                        Debug.Log($"Enemy attacked player for {damage} damage!");
                    }
                    else
                    {
                        // 혹시 BasePlayerStats가 아닌 다른 구현체가 있을 경우를 대비
                        var androidStats = player.GetComponent<AndroidPlayerStats>();
                        if (androidStats != null)
                        {
                            int damage = Mathf.RoundToInt(config.attackDamage);
                            androidStats.TakeDamage(damage);
                            
                            Debug.Log($"Enemy attacked Android player for {damage} damage!");
                        }
                    }
                }
            }
            else
            {
                state = NodeState.Failure;
                return state;
            }
        }
        
        attackTimer += Time.deltaTime;
        if (attackTimer >= 1f) // Attack duration
        {
            isAttacking = false;
            state = NodeState.Success;
            return state;
        }
        
        state = NodeState.Running;
        return state;
    }
}
