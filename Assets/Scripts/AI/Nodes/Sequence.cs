namespace BehaviorTree
{
    public class Sequence : CompositeNode
    {
        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false;
            
            foreach (var child in children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.Failure:
                        state = NodeState.Failure;
                        return state;
                    case NodeState.Success:
                        continue;
                    case NodeState.Running:
                        anyChildIsRunning = true;
                        continue;
                }
            }
            
            state = anyChildIsRunning ? NodeState.Running : NodeState.Success;
            return state;
        }
    }
}
