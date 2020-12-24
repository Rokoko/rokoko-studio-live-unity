#if UNITY_EDITOR

using UnityEditor;

/// <summary>
/// A simple class to inherit from when only minor tweaks to a component's inspector are required.
/// In such cases, a full custom inspector is normally overkill but, by inheriting from this class, custom tweaks become trivial.
/// 
/// To hide items from being drawn, simply override GetInvisibleInDefaultInspector, returning a string[] of fields to hide.
/// To draw/add extra GUI code/anything else you want before the default inspector is drawn, override OnBeforeDefaultInspector.
/// Similarly, override OnAfterDefaultInspector to draw GUI elements after the default inspector is drawn.
/// </summary>

namespace Rokoko.RokokoEditor
{
    public abstract class TweakableEditor : Editor
    {
        private static readonly string[] _emptyStringArray = new string[0];

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            OnBeforeDefaultInspector();
            DrawPropertiesExcluding(serializedObject, GetInvisibleInDefaultInspector());
            OnAfterDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnBeforeDefaultInspector()
        { }

        protected virtual void OnAfterDefaultInspector()
        { }

        protected virtual string[] GetInvisibleInDefaultInspector()
        {
            return _emptyStringArray;
        }
    }
}

#endif