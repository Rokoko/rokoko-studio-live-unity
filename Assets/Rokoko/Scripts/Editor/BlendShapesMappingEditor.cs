using Rokoko.Inputs;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Rokoko.Helper;

namespace Rokoko.UnityEditor
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
            Undo.RecordObject(blendshapesMapping, "Undo ActorCustomBoneMapping Changes");

            // Initialize Array
            if (blendshapesMapping.blendshapeNames.Count != BlendshapesArray.Length)
            {
                blendshapesMapping.blendshapeNames = new Dictionary<BlendShapes, string>(BlendshapesArray.Length);
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
                // Copy fiels from ARKit
                foreach (KeyValuePair<BlendShapes, string> pair in new Dictionary<BlendShapes, string>(blendshapesMapping.blendshapeNames))
                {
                    blendshapesMapping.blendshapeNames[pair.Key] = pair.Key.ToString();
                }
            }

            GUILayout.Space(10);

            // Draw a field for each Blendshape
            foreach(KeyValuePair<BlendShapes, string> pair in new Dictionary<BlendShapes, string>(blendshapesMapping.blendshapeNames))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(pair.Key.ToString().ToUpperFirstChar());
                blendshapesMapping.blendshapeNames[pair.Key] = EditorGUILayout.TextField(pair.Value.ToString());
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();

            // Draw standard fields
            //base.OnInspectorGUI();

        }
    }
}