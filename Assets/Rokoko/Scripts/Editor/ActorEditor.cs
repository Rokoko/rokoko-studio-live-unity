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
        bool notesToggle = false;

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

            EditorGUI.indentLevel++;

            notesToggle = EditorGUILayout.Foldout(notesToggle, "Notes");
            if (notesToggle)
            {
                GUI.skin.label.wordWrap = true;
                GUILayout.Label("This implements the behavior of a character controlled with a smartsuit.\n" +
                    "Auto select: If true smartsuit actor will connect to the first suit that it will receive message. If false you have to specify the suit ID\n" +
                    "Auto select unique: If true smartsuit will connect automatically to the first smartsuit that is not already connected to other SmartsuitActor\n" +
                    "HubID: If autoselect is false, the Actor will wait for the Smartsuit with the hub id you specify here to connect to.\n" +
                    "Use Humanoid bones: If true the smartsuit actor will use Unity's humanoid avatar to find the correct bone mapping with the smartsuit." +
                    "If this is false you have to specify the mapping.");
            }
            EditorGUI.indentLevel--;

            //actor.boneMapping = (Actor.BoneMappingEnum)EditorGUILayout.EnumPopup("BoneMapping", actor.boneMapping);
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
                if(actor.GetComponent<ActorCustomBoneMapping>() == null)
                    actor.customBoneMapping = actor.gameObject.AddComponent<ActorCustomBoneMapping>();
            }



            //if (Application.isPlaying)
            //{
            //    //GUILayout.BeginVertical(livePanelStyle);
            //    GUILayout.BeginVertical(warningStyle);
            //    if (actor.TTL > 0)
            //    {
            //        EditorGUILayout.LabelField("FPS: " + actor.FPS);
            //    }
            //    else
            //    {
            //        EditorGUILayout.LabelField("Actor is not connected");
            //    }
            //    EditorGUILayout.LabelField("Last Frame: " + actor.CurrentState.sensors.Length + " sensors");
            //    int count = 0;
            //    foreach (var s in actor.CurrentState.sensors)
            //    {
            //        string cmd = System.BitConverter.ToString(new byte[] { s.command });
            //        if (cmd == "25")
            //        {
            //            count++;
            //        }
            //    }
            //    if (count > 0)
            //    {
            //        EditorGUILayout.LabelField(count + " sensors detect metal");
            //    }
            //    else
            //    {
            //        EditorGUILayout.LabelField("Sensors status is good");
            //    }

            //    GUILayout.EndVertical();


            //    if (GUI.changed)
            //    {
            //        EditorUtility.SetDirty(target);
            //    }
            //    //GUILayout.Space(200);
            //}

            ////EditorGUILayout.RectField(lastRect);
            //DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();

            // Draw standard fields
            base.OnInspectorGUI();

        }
    }
}