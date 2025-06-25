using UnityEngine;
namespace EnemyAI.BehaviorTree
{
    public class SelectorNode : BehaviorNode
    {
        private BehaviorNode[] children;

        public SelectorNode(params BehaviorNode[] children)
        {
            this.children = children;
        }

        public override BehaviorResult Execute(EnemyAIController controller)
        {
            foreach (var child in children)
            {
                BehaviorResult result = child.Execute(controller);
                if (result == BehaviorResult.Success || result == BehaviorResult.Running)
                    return result;
            }
            return BehaviorResult.Failure;
        }
    }

    public class SequenceNode : BehaviorNode
    {
        private BehaviorNode[] children;
        public SequenceNode(params BehaviorNode[] children)
        {
            this.children = children;
        }

        public override BehaviorResult Execute(EnemyAIController controller)
        {
            foreach (var child in children)
            {
                BehaviorResult result = child.Execute(controller);
                if (result == BehaviorResult.Failure || result == BehaviorResult.Running)
                    return result;
            }
            return BehaviorResult.Success;
        }
    }

    public class ConditionNode : BehaviorNode
    {
        private System.Func<EnemyAIController, bool> condition;

        public ConditionNode(System.Func<EnemyAIController, bool> condition)
        {
            this.condition = condition;
        }
        public override BehaviorResult Execute(EnemyAIController controller)
        {
            return condition(controller) ? BehaviorResult.Success : BehaviorResult.Failure;
        }

        public class ActionNode : BehaviorNode
        {
            private System.Func<EnemyAIController, BehaviorResult> action;

            public ActionNode(System.Func<EnemyAIController, BehaviorResult> action)
            {
                this.action = action;
            }

            public override BehaviorResult Execute(EnemyAIController controller)
            {
                return action(controller);
            }
        }
    }
}