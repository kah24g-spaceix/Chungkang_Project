using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public interface IBlackboardTask
    {
        List<BlackboardKey> GetRequiredKeys();
        List<BlackboardKey> GetOptionalKeys();
        void InitializeBlackboardData(Blackboard blackboard);
    }
    public abstract class BlackboardTask : BehaviorNode, IBlackboardTask
    {
        protected Blackboard blackboard;
        
        public BlackboardTask(Blackboard blackboard)
        {
            this.blackboard = blackboard;
            InitializeBlackboardData(blackboard);
        }
        
        public abstract List<BlackboardKey> GetRequiredKeys();
        public virtual List<BlackboardKey> GetOptionalKeys() => new List<BlackboardKey>();
        public virtual void InitializeBlackboardData(Blackboard blackboard) { }
        
        protected virtual bool ValidateRequiredData()
        {
            foreach (var key in GetRequiredKeys())
            {
                if (!blackboard.HasValue(key))
                {
                    Debug.LogWarning($"Missing required blackboard data: {key}");
                    return false;
                }
            }
            return true;
        }
    }
}