#if UNITY_EDITOR

using Rokoko.Inputs;
using UnityEditor;
using UnityEngine;

namespace Rokoko.RokokoEditor
{
    [CustomEditor(typeof(HumanBoneMapping))]
    [CanEditMultipleObjects]
    public class HumanBoneMappingEditor : TweakableEditor
    {
        private HumanBodyBones[] _HumanBodyBonesArray;

        public HumanBodyBones[] HumanBodyBonesArray
        {
            get
            {
                if (_HumanBodyBonesArray == null)
                {
                    _HumanBodyBonesArray = new HumanBodyBones[(int)HumanBodyBones.LastBone];
                    for (int i = 0; i < _HumanBodyBonesArray.Length; i++)
                        _HumanBodyBonesArray[i] = (HumanBodyBones)i;
                }
                return _HumanBodyBonesArray;
            }
        }

        SerializedProperty customBodyBones;

        protected void OnEnable()
        {
            customBodyBones = serializedObject.FindProperty("customBodyBones");
        }

        // Stops showing the script field
        protected override string[] GetInvisibleInDefaultInspector()
        {
            return new[] { "m_Script" };
        }

        public override void OnInspectorGUI()
        {
            HumanBoneMapping boneMapping = (HumanBoneMapping)target;
            Undo.RecordObject(boneMapping, "Undo ActorCustomBoneMapping Changes");

            // Initialize Array
            if (boneMapping.customBodyBones.Length != HumanBodyBonesArray.Length)
            {
                boneMapping.customBodyBones = new Transform[HumanBodyBonesArray.Length];

                // SerializedObject rereferce needs to be updated
                return;
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Copy from Animator"))
            {
                Animator animator = boneMapping.GetComponent<Animator>();
                if (animator != null)
                {
                    for (int i = 0; i < HumanBodyBonesArray.Length; i++)
                    {
                        boneMapping.customBodyBones[i] = animator.GetBoneTransform(HumanBodyBonesArray[i]);
                    }
                }
            }

            GUILayout.Space(10);

            // Draw a field for each HumanBodyBone
            for (int i = 0; i < HumanBodyBonesArray.Length; i++)
            {
                SerializedProperty element = customBodyBones.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(element, new GUIContent(HumanBodyBonesArray[i].ToString()));
            }

            serializedObject.ApplyModifiedProperties();

            // Draw standard fields
            //base.OnInspectorGUI();

        }
    }
}

#endif