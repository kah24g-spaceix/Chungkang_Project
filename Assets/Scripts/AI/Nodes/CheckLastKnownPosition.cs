using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class CheckLastKnownPosition : BlackboardTask
{
    public CheckLastKnownPosition(Blackboard blackboard) : base(blackboard) { }

    public override List<BlackboardKey> GetRequiredKeys()
    {
        return new List<BlackboardKey>
        {
            BlackboardKey.IsPlayerVisible,
            BlackboardKey.LastKnownPlayerPosition
        };
    }

    public override NodeState Evaluate()
    {
        if (!ValidateRequiredData())
        {
            state = NodeState.Failure;
            return state;
        }

        bool isPlayerVisible = blackboard.GetValue<bool>(BlackboardKey.IsPlayerVisible);
        Vector3 lastKnownPosition = blackboard.GetValue<Vector3>(BlackboardKey.LastKnownPlayerPosition);

        if (!isPlayerVisible && lastKnownPosition != Vector3.zero)
        {
            Debug.Log($"[CheckLastKnownPosition] 마지막 위치 확인됨: {lastKnownPosition}");
            state = NodeState.Success;
        }
        else
        {
            state = NodeState.Failure;
        }

        return state;
    }
}
