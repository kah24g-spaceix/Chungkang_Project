using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class CheckPlayerInRange : BlackboardTask
{
    public CheckPlayerInRange(Blackboard blackboard) : base(blackboard) { }

    public override List<BlackboardKey> GetRequiredKeys()
    {
        return new List<BlackboardKey>
        {
            BlackboardKey.DistanceToPlayer,
            BlackboardKey.Config
        };
    }

    public override NodeState Evaluate()
    {
        if (!ValidateRequiredData())
        {
            state = NodeState.Failure;
            return state;
        }

        float distance = blackboard.GetValue<float>(BlackboardKey.DistanceToPlayer);
        var config = blackboard.GetValue<EnemyConfig>(BlackboardKey.Config);

        state = distance <= config.attackRange ? NodeState.Success : NodeState.Failure;
        return state;
    }
}
