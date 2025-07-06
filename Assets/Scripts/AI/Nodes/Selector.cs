namespace BehaviorTree
{
public class Selector : CompositeNode
    {
        private int currentChild = 0;

        public override NodeState Evaluate()
        {
            if (children.Count == 0)
            {
                state = NodeState.Failure;
                return state;
            }

            if (state != NodeState.Running)
                currentChild = 0;

            while (currentChild < children.Count)
            {
                var childState = children[currentChild].Evaluate();

                if (childState == NodeState.Running)
                {
                    state = NodeState.Running;
                    return state;
                }

                if (childState == NodeState.Success)
                {
                    currentChild = 0;
                    state = NodeState.Success;
                    return state;
                }

                // 실패한 자식은 다음으로
                currentChild++;
            }

            // 모든 자식 실패
            currentChild = 0;
            state = NodeState.Failure;
            return state;
        }
    }
}
