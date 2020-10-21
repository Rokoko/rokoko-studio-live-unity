using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/// @cond nodoc

namespace Rokoko.Smartsuit{
	
	[CustomEditor(typeof(SmartsuitActor))]
	public class SmartsuitActorEditor : SmartsuitAbstractEditor
    {

        SerializedProperty hubId;
        SerializedProperty autoSelect;
        SerializedProperty autoSelectUnique;
        SerializedProperty useHumanoid;
        SerializedProperty boneMapping;
        bool sensorsToggle = false;

        protected new void OnEnable()
        {
            base.OnEnable();
            hubId = serializedObject.FindProperty("HubID");
            autoSelect = serializedObject.FindProperty("autoSelect");
            autoSelectUnique = serializedObject.FindProperty("autoSelectUnique");
            useHumanoid = serializedObject.FindProperty("useHumanoidBones");
            boneMapping = serializedObject.FindProperty("boneMapping");
        }

        public override void OnInspectorGUI()
		{
            serializedObject.Update();
			SmartsuitActor actor = target as SmartsuitActor;
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

            EditorGUILayout.PropertyField(autoSelect);
            if (!actor.autoSelect)
            {
                EditorGUILayout.PropertyField(hubId);
            } else
            {
                EditorGUILayout.PropertyField(autoSelectUnique);
            }
            EditorGUILayout.PropertyField(useHumanoid);
            if (actor.useHumanoidBones){
                if (actor.GetComponent<Animator>() == null)
                {
                    
                    GUILayout.Label("<color=#FD515A>(!) When you use humanoid bones the object needs to have a humanoid avatar.</color>", warningStyle);
                } else if (!actor.GetComponent<Animator>().isHuman)
                {
                    EditorGUILayout.LabelField("(!) The avatar you are using is not humanoid.", warningStyle);
                }
            } else
            {
                EditorGUILayout.PropertyField(boneMapping, true);
            }
            if (Application.isPlaying)
            {
                GUILayout.BeginVertical(livePanelStyle);
                if (actor.TTL > 0)
                {
                    EditorGUILayout.LabelField("FPS: " + actor.FPS);
                }
                else
                {
                    EditorGUILayout.LabelField("Actor is not connected");
                }
                EditorGUILayout.LabelField("Last Frame: " + actor.CurrentState.sensors.Length + " sensors");
                int count = 0;
                foreach (var s in actor.CurrentState.sensors)
                {
                    string cmd = System.BitConverter.ToString(new byte[] { s.command });
                    if (cmd == "25")
                    {
                        count++;
                    }
                }
                if (count > 0)
                {
                    EditorGUILayout.LabelField(count + " sensors detect metal");
                }
                else
                {
                    EditorGUILayout.LabelField("Sensors status is good");
                }

                GUILayout.EndVertical();


                if (GUI.changed)
                {
                    EditorUtility.SetDirty(target);
                }
                //GUILayout.Space(200);
            }
            
            //EditorGUILayout.RectField(lastRect);
            serializedObject.ApplyModifiedProperties();
            //DrawDefaultInspector();
		}
    }
}
/// @endcond