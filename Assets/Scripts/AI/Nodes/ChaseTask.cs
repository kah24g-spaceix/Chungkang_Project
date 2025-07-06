using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class ChaseTask : BlackboardTask
{
    public ChaseTask(Blackboard blackboard) : base(blackboard) { }

    public override List<BlackboardKey> GetRequiredKeys()
    {
        return new List<BlackboardKey>
        {
            BlackboardKey.Player,
            BlackboardKey.Agent,
            BlackboardKey.Config
        };
    }

    public override List<BlackboardKey> GetOptionalKeys()
    {
        return new List<BlackboardKey>
        {
            BlackboardKey.Animator
        };
    }

    public override NodeState Evaluate()
    {
        if (!ValidateRequiredData())
        {
            state = NodeState.Failure;
            return state;
        }

        var player = blackboard.GetValue<Transform>(BlackboardKey.Player);
        var agent = blackboard.GetValue<NavMeshAgent>(BlackboardKey.Agent);
        var config = blackboard.GetValue<EnemyConfig>(BlackboardKey.Config);
        var animator = blackboard.GetValue<Animator>(BlackboardKey.Animator);

        agent.isStopped = false;
        agent.speed = config.runSpeed;
        agent.SetDestination(player.position);

        if (animator != null)
        {
            animator.SetFloat("Speed", agent.desiredVelocity.magnitude);
            animator.SetBool("IsChasing", true);
        }

        state = NodeState.Running;
        return state;
    }
}
