using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class CheckHealthLow : BlackboardTask
{
    public CheckHealthLow(Blackboard blackboard) : base(blackboard) { }

    public override List<BlackboardKey> GetRequiredKeys()
    {
        return new List<BlackboardKey>
        {
            BlackboardKey.Config,
            BlackboardKey.HealthPercentage
        };
    }

    public override NodeState Evaluate()
    {
        if (!ValidateRequiredData())
        {
            state = NodeState.Failure;
            return state;
        }

        var config = blackboard.GetValue<EnemyConfig>(BlackboardKey.Config);
        float healthPercentage = blackboard.GetValue<float>(BlackboardKey.HealthPercentage);

        state = healthPercentage <= config.fleeHealthPercentage
            ? NodeState.Success
            : NodeState.Failure;

        return state;
    }
}
