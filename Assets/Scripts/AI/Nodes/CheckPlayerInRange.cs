using BehaviorTree;

public class CheckPlayerInRange : BehaviorNode
{
    private Blackboard blackboard;
    
    public CheckPlayerInRange(Blackboard blackboard)
    {
        this.blackboard = blackboard;
    }
    
    public override NodeState Evaluate()
    {
        float distance = blackboard.GetValue<float>("distanceToPlayer");
        EnemyConfig config = blackboard.GetValue<EnemyConfig>("config");
        
        state = distance <= config.attackRange ? NodeState.Success : NodeState.Failure;
        return state;
    }
}
