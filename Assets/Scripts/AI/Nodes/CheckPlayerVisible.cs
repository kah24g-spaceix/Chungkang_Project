using BehaviorTree;

public class CheckPlayerVisible : BehaviorNode
{
    private Blackboard blackboard;
    
    public CheckPlayerVisible(Blackboard blackboard)
    {
        this.blackboard = blackboard;
    }
    
    public override NodeState Evaluate()
    {
        bool isVisible = blackboard.GetValue<bool>("isPlayerVisible");
        state = isVisible ? NodeState.Success : NodeState.Failure;
        return state;
    }
}
