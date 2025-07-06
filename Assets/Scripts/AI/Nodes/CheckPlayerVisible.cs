using System.Collections.Generic;
using BehaviorTree;

public class CheckPlayerVisible : BlackboardTask
{
    public CheckPlayerVisible(Blackboard blackboard) : base(blackboard) { }

    public override List<BlackboardKey> GetRequiredKeys()
    {
        return new List<BlackboardKey>
        {
            BlackboardKey.IsPlayerVisible
        };
    }

    public override NodeState Evaluate()
    {
        if (!ValidateRequiredData())
        {
            state = NodeState.Failure;
            return state;
        }

        bool isVisible = blackboard.GetValue<bool>(BlackboardKey.IsPlayerVisible);
        state = isVisible ? NodeState.Success : NodeState.Failure;
        return state;
    }
}