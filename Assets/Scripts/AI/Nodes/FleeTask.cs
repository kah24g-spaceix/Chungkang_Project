using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class FleeTask : BehaviorNode
{
    private Blackboard blackboard;
    private Vector3 fleeTarget;
    private bool hasFleeTarget = false;
    
    public FleeTask(Blackboard blackboard)
    {
        this.blackboard = blackboard;
    }
    
    public override NodeState Evaluate()
    {
        Transform player = blackboard.GetValue<Transform>("player");
        Transform transform = blackboard.GetValue<Transform>("transform");
        NavMeshAgent agent = blackboard.GetValue<NavMeshAgent>("agent");
        EnemyConfig config = blackboard.GetValue<EnemyConfig>("config");
        Animator animator = blackboard.GetValue<Animator>("animator");
        
        if (player == null)
        {
            state = NodeState.Failure;
            return state;
        }
        
        if (!hasFleeTarget)
        {
            // Calculate flee direction (opposite from player)
            Vector3 fleeDirection = (transform.position - player.position).normalized;
            fleeTarget = transform.position + fleeDirection * 10f; // Flee 10 units away
            
            // Make sure flee target is on navmesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(fleeTarget, out hit, 10f, NavMesh.AllAreas))
            {
                fleeTarget = hit.position;
            }
            
            hasFleeTarget = true;
        }
        
        // Move towards flee target
        agent.speed = config.runSpeed;
        agent.SetDestination(fleeTarget);
        
        // Update animation
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
            animator.SetBool("IsFleeing", true);
        }
        
        // Check if reached flee target
        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            hasFleeTarget = false;
            if (animator != null)
            {
                animator.SetBool("IsFleeing", false);
            }
            state = NodeState.Success;
            return state;
        }
        
        state = NodeState.Running;
        return state;
    }
}
