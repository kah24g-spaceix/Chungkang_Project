using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class ChaseTask : BehaviorNode
{
    private Blackboard blackboard;
    
    public ChaseTask(Blackboard blackboard)
    {
        this.blackboard = blackboard;
    }
    
    public override NodeState Evaluate()
    {
        Transform player = blackboard.GetValue<Transform>("player");
        NavMeshAgent agent = blackboard.GetValue<NavMeshAgent>("agent");
        EnemyConfig config = blackboard.GetValue<EnemyConfig>("config");
        Animator animator = blackboard.GetValue<Animator>("animator");
        
        if (player == null)
        {
            state = NodeState.Failure;
            return state;
        }
        
        // Update last known player position
        blackboard.SetValue("lastKnownPlayerPosition", player.position);
        
        // Set chase speed and destination
        agent.speed = config.runSpeed;
        agent.SetDestination(player.position);
        
        // Update animation
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
            animator.SetBool("IsChasing", true);
        }
        
        state = NodeState.Running;
        return state;
    }
}
