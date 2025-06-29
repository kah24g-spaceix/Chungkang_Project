using System.Collections.Generic;

namespace BehaviorTree
{
    public abstract class CompositeNode : BehaviorNode
    {
        protected List<BehaviorNode> children = new List<BehaviorNode>();
        
        public void AddChild(BehaviorNode child)
        {
            children.Add(child);
        }
        
        public void RemoveChild(BehaviorNode child)
        {
            children.Remove(child);
        }
        
        public void ClearChildren()
        {
            children.Clear();
        }
    }
}
