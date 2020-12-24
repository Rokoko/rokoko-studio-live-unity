#if UNITY_EDITOR

using Rokoko.Inputs;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rokoko.RokokoEditor
{
    [CustomEditor(typeof(PropColor))]
    [CanEditMultipleObjects]
    public class PropColorEditor : PropEditor
    {
        SerializedProperty meshRendererProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            meshRendererProperty = serializedObject.FindProperty("meshRenderer");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            PropColor propColor = (PropColor)target;
            serializedObject.Update();

            GUILayout.Space(10);

            EditorGUILayout.LabelField("Update mesh color", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(meshRendererProperty);
            if (GUILayout.Button("Self"))
            {
                propColor.meshRenderer = propColor.GetComponentInChildren<MeshRenderer>();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}

#endif