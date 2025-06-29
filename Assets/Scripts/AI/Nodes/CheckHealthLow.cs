using BehaviorTree;

public class CheckHealthLow : BehaviorNode
{
    private Blackboard blackboard;
    
    public CheckHealthLow(Blackboard blackboard)
    {
        this.blackboard = blackboard;
    }
    
    public override NodeState Evaluate()
    {
        float healthPercentage = blackboard.GetValue<float>("healthPercentage");
        EnemyConfig config = blackboard.GetValue<EnemyConfig>("config");
        
        state = healthPercentage <= config.fleeHealthPercentage ? NodeState.Success : NodeState.Failure;
        return state;
    }
}
