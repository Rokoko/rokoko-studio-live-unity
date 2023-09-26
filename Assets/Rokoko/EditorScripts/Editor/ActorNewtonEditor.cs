#if UNITY_EDITOR

using Rokoko.Inputs;
using UnityEditor;

namespace Rokoko.RokokoEditor
{
    [CustomEditor(typeof(ActorNewton))]
    [CanEditMultipleObjects]
    public class ActorNewtonEditor : ActorEditor { }
}

#endif