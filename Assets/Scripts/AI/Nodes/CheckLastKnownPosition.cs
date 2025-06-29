using UnityEngine;
using BehaviorTree;

public class CheckLastKnownPosition : BehaviorNode
{
    private Blackboard blackboard;
    
    public CheckLastKnownPosition(Blackboard blackboard)
    {
        this.blackboard = blackboard;
    }
    
    public override NodeState Evaluate()
    {
        bool isPlayerVisible = blackboard.GetValue<bool>("isPlayerVisible");
        Vector3 lastKnownPosition = blackboard.GetValue<Vector3>("lastKnownPlayerPosition");
        
        // If we can't see player but have a last known position
        state = !isPlayerVisible && lastKnownPosition != Vector3.zero ? NodeState.Success : NodeState.Failure;
        return state;
    }
}
