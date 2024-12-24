using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableKeyValue<K, V>
{
    public K Key;
    public V Value;

    public SerializableKeyValue() { }

    public SerializableKeyValue(K key, V value)
    {
        Key = key;
        Value = value;
    }
}


[Serializable]
public class SerializableDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<SerializableKeyValue<K, V>> _keyValueList = new List<SerializableKeyValue<K, V>>();

    public void OnBeforeSerialize()
    {
        _keyValueList.Clear();
        foreach (var kv in this)
        {
            _keyValueList.Add(new SerializableKeyValue<K, V>(kv.Key, kv.Value));
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();
        foreach (var kv in _keyValueList)
        {
            if (!this.ContainsKey(kv.Key))
            {
                this[kv.Key] = kv.Value;
            }
            else
            {
                Debug.LogWarning($"Duplicate key found during deserialization: {kv.Key}");
            }
        }
    }
}