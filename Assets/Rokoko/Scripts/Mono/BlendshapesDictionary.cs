using Rokoko.Core;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Create a simple serialized version of a Dictionary in order to able to persist in Editor play mode.
/// </summary>
[System.Serializable]
public class BlendshapesDictionary
{
    public List<BlendShapes> keys = new List<BlendShapes>();
    public List<string> values = new List<string>();

    public void Add(BlendShapes key, string value)
    {
        if (keys.Contains(key))
            throw new System.Exception("Key already exists");
        keys.Add(key);
        values.Add(value);
    }

    public string this[BlendShapes key]
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

    public KeyValuePair<BlendShapes, string> this[int index]
    {
        get
        {
            if (keys.Count < index)
                throw new System.IndexOutOfRangeException();
            return new KeyValuePair<BlendShapes, string>(keys[index], values[index]);
        }
    }

    public int Count => keys.Count;
}
