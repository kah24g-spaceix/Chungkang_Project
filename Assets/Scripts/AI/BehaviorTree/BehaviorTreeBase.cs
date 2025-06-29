using UnityEngine;

namespace BehaviorTree
{
    public abstract class BehaviorTreeBase : MonoBehaviour
    {
        private BehaviorNode rootNode;
        
        protected virtual void Start()
        {
            rootNode = SetupTree();
        }
        
        protected virtual void Update()
        {
            if (rootNode != null)
            {
                rootNode.Evaluate();
            }
        }
        
        protected abstract BehaviorNode SetupTree();
    }
}
