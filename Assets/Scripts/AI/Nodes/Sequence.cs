namespace BehaviorTree
{
    public class Sequence : CompositeNode
    {
        private int currentChild = 0;

        public override NodeState Evaluate()
        {
            // 자식이 없으면 실패
            if (children.Count == 0)
            {
                state = NodeState.Failure;
                return state;
            }

            // 처음 진입할 때 초기화
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

                if (childState == NodeState.Failure)
                {
                    // 실패 시 리셋 후 실패 반환
                    currentChild = 0;
                    state = NodeState.Failure;
                    return state;
                }

                // 성공했으면 다음 자식으로
                currentChild++;
            }

            // 모든 자식 성공
            currentChild = 0;
            state = NodeState.Success;
            return state;
        }
    }
}
