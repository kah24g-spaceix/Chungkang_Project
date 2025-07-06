using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class FleeTask : BlackboardTask
{
    public FleeTask(Blackboard blackboard) : base(blackboard) { }

    public override List<BlackboardKey> GetRequiredKeys()
    {
        return new List<BlackboardKey>
        {
            BlackboardKey.Player,
            BlackboardKey.Transform,
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

    public override void InitializeBlackboardData(Blackboard blackboard)
    {
        if (!blackboard.HasValue(BlackboardKey.HasFleeTarget))
            blackboard.SetValue(BlackboardKey.HasFleeTarget, false);
        
        if (!blackboard.HasValue(BlackboardKey.FleeTarget))
            blackboard.SetValue(BlackboardKey.FleeTarget, Vector3.zero);
    }

    public override NodeState Evaluate()
    {
        if (!ValidateRequiredData())
        {
            state = NodeState.Failure;
            return state;
        }

        var player = blackboard.GetValue<Transform>(BlackboardKey.Player);
        var self = blackboard.GetValue<Transform>(BlackboardKey.Transform);
        var agent = blackboard.GetValue<NavMeshAgent>(BlackboardKey.Agent);
        var config = blackboard.GetValue<EnemyConfig>(BlackboardKey.Config);
        var animator = blackboard.GetValue<Animator>(BlackboardKey.Animator);

        Vector3 selfPos = self.position;
        bool hasFlee = blackboard.GetValue<bool>(BlackboardKey.HasFleeTarget);
        Vector3 target = blackboard.GetValue<Vector3>(BlackboardKey.FleeTarget);

        if (!hasFlee)
        {
            Vector3 dir = (selfPos - player.position).normalized;
            Vector3 desired = selfPos + dir * config.fleeDistance;
            
            if (NavMesh.SamplePosition(desired, out NavMeshHit hit, config.fleeSampleRadius, NavMesh.AllAreas))
                target = hit.position;
            else
                target = desired;

            blackboard.SetValue(BlackboardKey.FleeTarget, target);
            blackboard.SetValue(BlackboardKey.HasFleeTarget, true);
        }

        agent.speed = config.runSpeed;
        agent.SetDestination(target);

        if (animator != null)
        {
            animator.SetFloat("Speed", agent.desiredVelocity.magnitude);
            animator.SetBool("IsFleeing", true);
        }

        if (!agent.pathPending && agent.remainingDistance <= config.fleeStopDistance)
        {
            blackboard.SetValue(BlackboardKey.HasFleeTarget, false);
            if (animator != null)
                animator.SetBool("IsFleeing", false);
            
            state = NodeState.Success;
            return state;
        }

        state = NodeState.Running;
        return state;
    }
}
