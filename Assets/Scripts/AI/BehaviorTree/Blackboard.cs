using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class Blackboard
    {
        private Dictionary<BlackboardKey, object> data = new Dictionary<BlackboardKey, object>();
        private Dictionary<string, BlackboardKey> stringToKeyMap = new Dictionary<string, BlackboardKey>();
        
        public Blackboard()
        {
            InitializeStringMapping();
        }
        
        private void InitializeStringMapping()
        {
            foreach (BlackboardKey key in System.Enum.GetValues(typeof(BlackboardKey)))
            {
                stringToKeyMap[key.ToString()] = key;
            }
        }
        
        public void SetValue<T>(BlackboardKey key, T value)
        {
            if (data.ContainsKey(key))
                data[key] = value;
            else
                data.Add(key, value);
        }
        
        public T GetValue<T>(BlackboardKey key)
        {
            if (data.ContainsKey(key))
                return (T)data[key];
            return default(T);
        }
        
        public bool HasValue(BlackboardKey key)
        {
            return data.ContainsKey(key);
        }
        
        public void RemoveValue(BlackboardKey key)
        {
            if (data.ContainsKey(key))
                data.Remove(key);
        }
        
        // 하위 호환성을 위한 string 메서드들
        [System.Obsolete("Use BlackboardKey enum instead of string keys")]
        public void SetValue<T>(string key, T value)
        {
            if (stringToKeyMap.TryGetValue(key, out BlackboardKey enumKey))
                SetValue(enumKey, value);
            else
                Debug.LogWarning($"Unknown blackboard key: {key}");
        }
        
        [System.Obsolete("Use BlackboardKey enum instead of string keys")]
        public T GetValue<T>(string key)
        {
            if (stringToKeyMap.TryGetValue(key, out BlackboardKey enumKey))
                return GetValue<T>(enumKey);
            return default(T);
        }
    }
}
