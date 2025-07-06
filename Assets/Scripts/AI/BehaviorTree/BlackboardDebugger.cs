using UnityEngine;
using BehaviorTree;
[System.Serializable]
public class BlackboardDebugger
{
    public static void LogBlackboardState(Blackboard blackboard, BlackboardConfig config)
    {
        if (config == null) return;
        
        Debug.Log("=== Blackboard State ===");
        foreach (var entry in config.GetEntries())
        {
            bool hasValue = blackboard.HasValue(entry.key);
            string status = entry.isRequired ? (hasValue ? "✓" : "✗") : (hasValue ? "○" : "-");
            Debug.Log($"{status} {entry.key}: {entry.description}");
        }
    }
}
