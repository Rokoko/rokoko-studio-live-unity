﻿using Rokoko.Inputs;
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

        protected void OnEnable()
        {
            boneMapping = serializedObject.FindProperty("boneMapping");
            animatorProperty = serializedObject.FindProperty("animator");
            customBoneMappingProperty = serializedObject.FindProperty("customBoneMapping");
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

            EditorGUILayout.HelpBox("Bone mapping is used to convert a Studio Actor to any custom character hierarchy", MessageType.Info);
            EditorGUILayout.PropertyField(boneMapping);
            if (actor.boneMapping == Actor.BoneMappingEnum.Animator)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(animatorProperty);
                if (GUILayout.Button("Self"))
                {
                    actor.animator = actor.GetComponent<Animator>();
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
                EditorGUILayout.PropertyField(customBoneMappingProperty);
                if (actor.GetComponent<HumanBoneMapping>() == null)
                {
                    Undo.RecordObject(actor.gameObject, "Undo Actor Changes");
                    actor.customBoneMapping = Undo.AddComponent(actor.gameObject, typeof(HumanBoneMapping)) as HumanBoneMapping;
                }
            }

            serializedObject.ApplyModifiedProperties();

            // Draw standard fields
            base.OnInspectorGUI();
        }
    }
}