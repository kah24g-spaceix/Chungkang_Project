using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using System;

[CreateAssetMenu(fileName = "BlackboardConfig", menuName = "AI/Blackboard Config")]
public class BlackboardConfig : ScriptableObject
{
    [System.Serializable]
    public class BlackboardEntry
    {
        public BlackboardKey key;
        [TextArea(2, 4)]
        public string description;
        public bool isRequired;
        public EntryType entryType;
        
        [Header("Default Values")]
        public bool boolValue;
        public int intValue;
        public float floatValue;
        public string stringValue;
        public Vector3 vectorValue;
    }
    
    public enum EntryType
    {
        Transform,
        NavMeshAgent,
        Animator,
        EnemyConfig,
        AndroidPlayerStats,
        Bool,
        Int,
        Float,
        String,
        Vector3
    }
    
    [SerializeField] private List<BlackboardEntry> entries = new List<BlackboardEntry>();
    
    public List<BlackboardEntry> GetEntries() => entries;
    
    public bool IsKeyRequired(BlackboardKey key)
    {
        var entry = entries.Find(e => e.key == key);
        return entry?.isRequired ?? false;
    }
    
    public BlackboardEntry GetEntry(BlackboardKey key)
    {
        return entries.Find(e => e.key == key);
    }
    
    public void ValidateBlackboard(Blackboard blackboard)
    {
        foreach (var entry in entries)
        {
            if (entry.isRequired && !blackboard.HasValue(entry.key))
            {
                Debug.LogWarning($"Required blackboard key missing: {entry.key} - {entry.description}");
            }
        }
    }
    
    public void InitializeDefaultValues(Blackboard blackboard)
    {
        foreach (var entry in entries)
        {
            if (!blackboard.HasValue(entry.key))
            {
                SetDefaultValue(blackboard, entry);
            }
        }
    }
    
    private void SetDefaultValue(Blackboard blackboard, BlackboardEntry entry)
    {
        switch (entry.entryType)
        {
            case EntryType.Bool:
                blackboard.SetValue(entry.key, entry.boolValue);
                break;
            case EntryType.Int:
                blackboard.SetValue(entry.key, entry.intValue);
                break;
            case EntryType.Float:
                blackboard.SetValue(entry.key, entry.floatValue);
                break;
            case EntryType.String:
                blackboard.SetValue(entry.key, entry.stringValue);
                break;
            case EntryType.Vector3:
                blackboard.SetValue(entry.key, entry.vectorValue);
                break;
        }
    }
}
