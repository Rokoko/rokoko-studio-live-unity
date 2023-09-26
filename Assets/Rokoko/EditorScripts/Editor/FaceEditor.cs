#if UNITY_EDITOR

using Rokoko.Helper;
using Rokoko.Inputs;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Rokoko.RokokoEditor
{
    [CustomEditor(typeof(Face))]
    [CanEditMultipleObjects]
    public class FaceEditor : TweakableEditor
    {
        private static int blendshapesCount;

        SerializedProperty blendshapeMappingProperty;
        SerializedProperty blendshapeCustomMapProperty;
        SerializedProperty meshRendererProperty;

        protected void OnEnable()
        {
            blendshapeMappingProperty = serializedObject.FindProperty("blendshapeMapping");
            blendshapeCustomMapProperty = serializedObject.FindProperty("blendshapeCustomMap");
            meshRendererProperty = serializedObject.FindProperty("meshRenderer");
            blendshapesCount = (int)Core.BlendShapes.size;

            Face face = (Face)target;
            face.meshRenderer = face.GetComponentInChildren<SkinnedMeshRenderer>();
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
            EditorGUILayout.PropertyField(blendshapeMappingProperty);
            if (face.blendshapeMapping == Face.FaceMappingEnum.ARKit)
            {
                // Do nothing
            }
            else
            {
                EditorGUILayout.PropertyField(blendshapeCustomMapProperty);
                if (face.GetComponent<BlendShapesMapping>() == null)
                {
                    face.blendshapeCustomMap = Undo.AddComponent(face.gameObject, typeof(BlendShapesMapping)) as BlendShapesMapping;
                }
            }

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PropertyField(meshRendererProperty);
                if (GUILayout.Button("Find in Hierarchy"))
                {
                    face.meshRenderer = face.GetComponentInChildren<SkinnedMeshRenderer>();
                }
            }
            EditorGUILayout.EndHorizontal();


            if (face.meshRenderer == null)
            {
                EditorGUILayout.HelpBox("You need to assign a valid SkinnedMeshRenderer", MessageType.Error);
            }
            else if (face.meshRenderer.sharedMesh.blendShapeCount == 0)
            {
                EditorGUILayout.HelpBox("Assigned SkinnedMeshRenderer doesn't have any blendshapes", MessageType.Warning);
            }
            else if (face.meshRenderer.sharedMesh.blendShapeCount > 0)
            {
                if (face.meshRenderer.sharedMesh.blendShapeCount < blendshapesCount)
                {
                    StringBuilder message = new StringBuilder();
                    message.AppendLine($"Face supports {blendshapesCount} blendshapes, but found {face.meshRenderer.sharedMesh.blendShapeCount} on SkinnedMeshRenderer\n");
                    List<string> missingBlendshapes = face.meshRenderer.sharedMesh.GetAllMissingBlendshapes();
                    message.AppendLine("Missing blendshapes:");
                    for (int i = 0; i < missingBlendshapes.Count; i++)
                    {
                        message.AppendLine(missingBlendshapes[i]);
                    }
                    EditorGUILayout.HelpBox(message.ToString(), MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.HelpBox($"Found {face.meshRenderer.sharedMesh.blendShapeCount} blendshapes on SkinnedMeshRenderer", MessageType.Info);
                }
            }

            serializedObject.ApplyModifiedProperties();

            // Draw standard fields
            base.OnInspectorGUI();
        }
    }
}

#endif