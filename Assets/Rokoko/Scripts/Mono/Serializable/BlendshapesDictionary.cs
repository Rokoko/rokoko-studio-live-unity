using Rokoko.Core;

/// <summary>
/// Create a simple serialized version of a Dictionary in order to able to persist in Editor play mode.
/// </summary>
[System.Serializable]
public class BlendshapesDictionary : SerializableDictionary<BlendShapes, string> { }
