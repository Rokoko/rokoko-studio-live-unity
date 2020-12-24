using Rokoko.Core;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Create a simple serialized version of a Dictionary in order to able to persist in Editor play mode.
/// </summary>
[System.Serializable]
public abstract class SerializableDictionary<TKey, TValue>
{
    public List<TKey> keys = new List<TKey>();
    public List<TValue> values = new List<TValue>();

    public void Add(TKey key, TValue value)
    {
        if (keys.Contains(key))
            throw new System.Exception("Key already exists");
        keys.Add(key);
        values.Add(value);
    }

    public TValue this[TKey key]
    {
        get
        {
            if (!keys.Contains(key))
                throw new System.Exception("Key doesn't exists");
            return values[keys.IndexOf(key)];
        }
        set
        {
            if (!keys.Contains(key))
                throw new System.Exception("Key doesn't exists");

            int index = keys.IndexOf(key);
            values[index] = value;
        }

    }

    public KeyValuePair<TKey, TValue> this[int index]
    {
        get
        {
            if (keys.Count < index)
                throw new System.IndexOutOfRangeException();
            return new KeyValuePair<TKey, TValue>(keys[index], values[index]);
        }
    }

    public bool Contains(TKey key)
    {
        return keys.Contains(key);
    }
    
    public  void Clear()
    {
        keys.Clear();
        values.Clear();
    }

    public int Count => keys.Count;
}
