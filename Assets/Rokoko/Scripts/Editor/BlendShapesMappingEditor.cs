#if UNITY_EDITOR

using Rokoko.Inputs;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Rokoko.Helper;
using Rokoko.Core;

namespace Rokoko.RokokoEditor
{
    [CustomEditor(typeof(BlendShapesMapping))]
    [CanEditMultipleObjects]
    public class BlendShapesMappingEditor : TweakableEditor
    {
        private BlendShapes[] _BlendshapesArray = null;
        public BlendShapes[] BlendshapesArray
        {
            get
            {
                if (_BlendshapesArray == null)
                {
                    _BlendshapesArray = new BlendShapes[(int)BlendShapes.size];
                    for (int i = 0; i < _BlendshapesArray.Length; i++)
                    {
                        _BlendshapesArray[i] = (BlendShapes)i;
                    }
                }

                return _BlendshapesArray;
            }
        }


        SerializedProperty blendshapeNames;

        protected void OnEnable()
        {
            blendshapeNames = serializedObject.FindProperty("blendshapeNames");
        }

        // Stops showing the script field
        protected override string[] GetInvisibleInDefaultInspector()
        {
            return new[] { "m_Script" };
        }

        public override void OnInspectorGUI()
        {
            BlendShapesMapping blendshapesMapping = (BlendShapesMapping)target;
            serializedObject.Update();

            Undo.RecordObject(blendshapesMapping, "Undo ActorCustomBoneMapping Changes");

            // Initialize Array
            if (blendshapesMapping.blendshapeNames.Count != BlendshapesArray.Length)
            {
                blendshapesMapping.blendshapeNames = new BlendshapesDictionary();
                for (int i = 0; i < BlendshapesArray.Length; i++)
                {
                    blendshapesMapping.blendshapeNames.Add(BlendshapesArray[i], "");
                }

                // SerializedObject rereferce needs to be updated
                return;
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Copy from ARKit"))
            {
                // Copy fields from ARKit
                for (int i = 0; i < blendshapesMapping.blendshapeNames.Count; i++)
                {
                    KeyValuePair<BlendShapes, string> keyPair = blendshapesMapping.blendshapeNames[i];
                    blendshapesMapping.blendshapeNames[keyPair.Key] = keyPair.Key.ToString();
                }
            }

            GUILayout.Space(10);

            // Draw a field for each Blendshape
            for (int i = 0; i < blendshapesMapping.blendshapeNames.Count; i++)
            {
                KeyValuePair<BlendShapes, string> keyPair = blendshapesMapping.blendshapeNames[i];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(keyPair.Key.ToString().ToUpperFirstChar());
                blendshapesMapping.blendshapeNames[keyPair.Key] = EditorGUILayout.TextField(keyPair.Value.ToString());
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();

            // Draw standard fields
            //base.OnInspectorGUI();
        }
    }
}

#endif