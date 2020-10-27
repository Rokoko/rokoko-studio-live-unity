using Rokoko.Inputs;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rokoko.UnityEditor
{
    [CustomEditor(typeof(Face))]
    [CanEditMultipleObjects]
    public class FaceEditor : TweakableEditor
    {
        SerializedProperty blendshapeMapping;
        SerializedProperty blendshapeCustomMap;

        protected void OnEnable()
        {
            blendshapeMapping = serializedObject.FindProperty("blendshapeMapping");
            blendshapeCustomMap = serializedObject.FindProperty("blendshapeCustomMap");
        }

        // Stops showing the script field
        protected override string[] GetInvisibleInDefaultInspector()
        {
            return new[] { "m_Script" };
        }

        public override void OnInspectorGUI()
        {
            Face face = (Face)target;
            serializedObject.Update();

            Undo.RecordObject(face, "Undo Face Changes");

            EditorGUILayout.HelpBox("Blendshape mapping is used to convert a Studio Face to any custom character blendshapes", MessageType.Info);
            EditorGUILayout.PropertyField(blendshapeMapping);
            if (face.blendshapeMapping == Face.FaceMappingEnum.ARKit)
            {
                // Do nothing
            }
            else
            {
                EditorGUILayout.PropertyField(blendshapeCustomMap);
                if (face.GetComponent<BlendShapesMapping>() == null)
                {
                    Undo.RecordObject(face.gameObject, "Undo Face Changes");
                    face.blendshapeCustomMap = Undo.AddComponent(face.gameObject, typeof(BlendShapesMapping)) as BlendShapesMapping;
                }
            }

            serializedObject.ApplyModifiedProperties();

            // Draw standard fields
            base.OnInspectorGUI();
        }
    }
}