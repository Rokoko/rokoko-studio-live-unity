using Rokoko.Inputs;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rokoko.UnityEditor
{
    [CustomEditor(typeof(Actor))]
    [CanEditMultipleObjects]
    public class ActorEditor : TweakableEditor
    {
        SerializedProperty boneMapping;
        SerializedProperty animatorProperty;
        SerializedProperty customBoneMappingProperty;
        SerializedProperty profileNameProperty;
        SerializedProperty faceProperty;

        protected void OnEnable()
        {
            boneMapping = serializedObject.FindProperty("boneMapping");
            animatorProperty = serializedObject.FindProperty("animator");
            customBoneMappingProperty = serializedObject.FindProperty("customBoneMapping");
            profileNameProperty = serializedObject.FindProperty("profileName");
            faceProperty = serializedObject.FindProperty("face");
        }

        // Stops showing the script field
        protected override string[] GetInvisibleInDefaultInspector()
        {
            return new[] { "m_Script" };
        }

        public override void OnInspectorGUI()
        {
            Actor actor = (Actor)target;
            serializedObject.Update();

            Undo.RecordObject(actor, "Undo Actor Changes");

            EditorGUILayout.HelpBox("Profile name allows you to override any target from studio", MessageType.Info);
            EditorGUILayout.PropertyField(profileNameProperty);

            GUILayout.Space(10);

            EditorGUILayout.HelpBox("Bone mapping is used to convert a Studio Actor to any custom character hierarchy", MessageType.Info);
            EditorGUILayout.PropertyField(boneMapping);
            if (actor.boneMapping == Actor.BoneMappingEnum.Animator)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(animatorProperty);
                if (GUILayout.Button("Self"))
                {
                    actor.animator = actor.GetComponentInChildren<Animator>();
                }
                EditorGUILayout.EndHorizontal();

                if (actor.animator == null)
                {
                    EditorGUILayout.HelpBox("Please select a valid Animator", MessageType.Warning);
                }
                else if (!actor.animator.isHuman)
                {
                    EditorGUILayout.HelpBox("The avatar you are using is not humanoid.", MessageType.Warning);
                }
            }
            else
            {
                if (actor.GetComponent<HumanBoneMapping>() == null)
                {
                    Undo.RecordObject(actor.gameObject, "Undo Actor Changes");
                    actor.customBoneMapping = Undo.AddComponent(actor.gameObject, typeof(HumanBoneMapping)) as HumanBoneMapping;
                }
                else if(actor.customBoneMapping == null)
                {
                    actor.customBoneMapping = actor.GetComponent<HumanBoneMapping>();
                }
            }

            GUILayout.Space(10);

            EditorGUILayout.LabelField("Actor Face (Optional)", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(faceProperty);
            if (GUILayout.Button("Self"))
            {
                actor.face = actor.GetComponentInChildren<Face>();
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();

            // Draw standard fields
            base.OnInspectorGUI();
        }
    }
}