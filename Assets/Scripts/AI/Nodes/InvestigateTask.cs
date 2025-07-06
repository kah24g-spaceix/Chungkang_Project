using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class InvestigateTask : BlackboardTask
{
    public InvestigateTask(Blackboard blackboard) : base(blackboard) { }

    public override List<BlackboardKey> GetRequiredKeys()
    {
        return new List<BlackboardKey>
        {
            BlackboardKey.LastKnownPlayerPosition,
            BlackboardKey.Agent,
            BlackboardKey.Config,
            BlackboardKey.Transform
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
        if (!blackboard.HasValue(BlackboardKey.HasReachedPosition))
            blackboard.SetValue(BlackboardKey.HasReachedPosition, false);
        
        if (!blackboard.HasValue(BlackboardKey.InvestigateTimer))
            blackboard.SetValue(BlackboardKey.InvestigateTimer, 0f);
    }

    public override NodeState Evaluate()
    {
        if (!ValidateRequiredData())
        {
            state = NodeState.Failure;
            return state;
        }

        var lastKnownPos = blackboard.GetValue<Vector3>(BlackboardKey.LastKnownPlayerPosition);
        var agent = blackboard.GetValue<NavMeshAgent>(BlackboardKey.Agent);
        var config = blackboard.GetValue<EnemyConfig>(BlackboardKey.Config);
        var transform = blackboard.GetValue<Transform>(BlackboardKey.Transform);
        var animator = blackboard.GetValue<Animator>(BlackboardKey.Animator);

        if (lastKnownPos == Vector3.zero)
        {
            state = NodeState.Failure;
            return state;
        }

        blackboard.SetValue(BlackboardKey.SightState, SightState.Investigating);

        bool hasReached = blackboard.GetValue<bool>(BlackboardKey.HasReachedPosition);
        float timer = blackboard.GetValue<float>(BlackboardKey.InvestigateTimer);

        if (!hasReached)
        {
            agent.isStopped = false;
            agent.speed = config.walkSpeed;
            agent.SetDestination(lastKnownPos);

            if (animator != null)
            {
                animator.SetFloat("Speed", agent.desiredVelocity.magnitude);
                animator.SetBool("IsInvestigating", true);
            }

            if (!agent.pathPending && agent.remainingDistance < config.investigationArrivalDistance)
            {
                blackboard.SetValue(BlackboardKey.HasReachedPosition, true);
                blackboard.SetValue(BlackboardKey.InvestigateTimer, 0f);
                agent.isStopped = true;
            }
        }
        else
        {
            timer += Time.deltaTime;
            blackboard.SetValue(BlackboardKey.InvestigateTimer, timer);

            float rotSpeed = config.investigationRotationSpeed;
            transform.Rotate(0f, rotSpeed * Time.deltaTime, 0f);

            if (timer >= config.investigationTime)
            {
                blackboard.SetValue(BlackboardKey.LastKnownPlayerPosition, Vector3.zero);
                blackboard.SetValue(BlackboardKey.HasReachedPosition, false);
                blackboard.SetValue(BlackboardKey.InvestigateTimer, 0f);

                if (animator != null)
                    animator.SetBool("IsInvestigating", false);

                state = NodeState.Success;
                return state;
            }
        }

        state = NodeState.Running;
        return state;
    }
}
