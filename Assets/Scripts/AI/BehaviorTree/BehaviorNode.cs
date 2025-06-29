using UnityEngine;

namespace BehaviorTree
{
    public enum NodeState
    {
        Running,
        Success,
        Failure
    }

    public abstract class BehaviorNode
    {
        protected NodeState state;
        
        public NodeState State => state;
        
        public abstract NodeState Evaluate();
        
        protected virtual void OnStart() { }
        protected virtual void OnEnd() { }
    }
}
