using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class InvestigateTask : BehaviorNode
{
    private Blackboard blackboard;
    private float investigateTimer = 0f;
    private bool hasReachedPosition = false;
    
    public InvestigateTask(Blackboard blackboard)
    {
        this.blackboard = blackboard;
    }
    
    public override NodeState Evaluate()
    {
        Vector3 lastKnownPosition = blackboard.GetValue<Vector3>("lastKnownPlayerPosition");
        NavMeshAgent agent = blackboard.GetValue<NavMeshAgent>("agent");
        EnemyConfig config = blackboard.GetValue<EnemyConfig>("config");
        Transform transform = blackboard.GetValue<Transform>("transform");
        Animator animator = blackboard.GetValue<Animator>("animator");
        
        if (lastKnownPosition == Vector3.zero)
        {
            state = NodeState.Failure;
            return state;
        }
        
        // 수색 상태로 변경 (시야 확장 유지)
        blackboard.SetValue("sightState", SightState.Investigating);
        
        if (!hasReachedPosition)
        {
            agent.speed = config.walkSpeed;
            agent.SetDestination(lastKnownPosition);
            
            if (animator != null)
            {
                animator.SetFloat("Speed", agent.velocity.magnitude);
                animator.SetBool("IsInvestigating", true);
            }
            
            if (!agent.pathPending && agent.remainingDistance < 1f)
            {
                hasReachedPosition = true;
                investigateTimer = 0f;
            }
        }
        else
        {
            investigateTimer += Time.deltaTime;
            
            // 주변을 둘러보며 수색
            float rotationSpeed = config.investigationRotationSpeed;
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            
            if (investigateTimer >= config.investigationTime)
            {
                // 수색 완료 - 마지막 위치 초기화
                blackboard.SetValue("lastKnownPlayerPosition", Vector3.zero);
                hasReachedPosition = false;
                
                if (animator != null)
                {
                    animator.SetBool("IsInvestigating", false);
                }
                
                state = NodeState.Success;
                return state;
            }
        }
        
        state = NodeState.Running;
        return state;
    }
}
