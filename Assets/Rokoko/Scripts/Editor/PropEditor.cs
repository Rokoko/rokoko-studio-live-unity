#if UNITY_EDITOR

using Rokoko.Inputs;
using UnityEditor;

namespace Rokoko.RokokoEditor
{
    [CustomEditor(typeof(Prop))]
    [CanEditMultipleObjects]
    public class PropEditor : TweakableEditor
    {
        SerializedProperty propNameProperty;

        protected virtual void OnEnable()
        {
            propNameProperty = serializedObject.FindProperty("propName");
        }

        // Stops showing the script field
        protected override string[] GetInvisibleInDefaultInspector()
        {
            return new[] { "m_Script" };
        }

        public override void OnInspectorGUI()
        {
            Prop prop = (Prop)target;
            serializedObject.Update();

            Undo.RecordObject(prop, "Undo Prop Changes");

            EditorGUILayout.HelpBox("Prop name allows you to override any prop target from studio", MessageType.Info);
            EditorGUILayout.PropertyField(propNameProperty);

            serializedObject.ApplyModifiedProperties();

            // Draw standard fields
            base.OnInspectorGUI();
        }
    }
}

#endif