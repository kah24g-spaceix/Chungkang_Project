namespace BehaviorTree
{
    public class Selector : CompositeNode
    {
        public override NodeState Evaluate()
        {
            foreach (var child in children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.Failure:
                        continue;
                    case NodeState.Success:
                        state = NodeState.Success;
                        return state;
                    case NodeState.Running:
                        state = NodeState.Running;
                        return state;
                }
            }
            
            state = NodeState.Failure;
            return state;
        }
    }
}
