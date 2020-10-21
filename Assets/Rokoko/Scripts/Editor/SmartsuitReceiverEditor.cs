using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/// @cond nodoc
namespace Rokoko.Smartsuit.Networking
{
    [CustomEditor(typeof(SmartsuitReceiver), true)]
    public class SmartsuitReceiverEditor : SmartsuitAbstractEditor
    {
        SerializedProperty autoStart;
        SerializedProperty portRangeStart;
        SerializedProperty portRangeEnd;
        SerializedProperty listenSuit;
        bool toggleSuits = true;

        protected new void OnEnable()
        {
            base.OnEnable();
            autoStart = serializedObject.FindProperty("autoStart");
            portRangeStart = serializedObject.FindProperty("portRangeStart");
            portRangeEnd = serializedObject.FindProperty("portRangeEnd");
            listenSuit = serializedObject.FindProperty("listenSuit");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SmartsuitReceiver receiver = target as SmartsuitReceiver;
            EditorGUI.indentLevel++;
            notesToggle = EditorGUILayout.Foldout(notesToggle, "Notes");
            EditorGUI.indentLevel--;
            if (notesToggle)
            {

                //EditorStyles.label.wordWrap = true;
                GUI.skin.label.wordWrap = true;
                GUILayout.Label("This component provides the interface to listen for Smartsuits live data. There must be one component of this type active in the scene.");
                GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
            }
            //EditorStyles.label.wordWrap = false;
            EditorGUILayout.PropertyField(autoStart);
            EditorGUILayout.PropertyField(portRangeStart);
            EditorGUILayout.PropertyField(portRangeEnd);
            EditorGUILayout.PropertyField(listenSuit);

            var allSuitReceivers = FindObjectsOfType<SmartsuitReceiver>();
            if (allSuitReceivers.Length > 1)
            {
                GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
                GUILayout.Label("<color=#FD515A>(!) You have two SmartsuitReceiver components active in the scene.</color>", warningStyle);
            }

            if (Application.isPlaying)
            {
                GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
                GUILayout.BeginVertical(livePanelStyle);
                EditorGUILayout.LabelField("FPS: " + receiver.FPS);
                EditorGUILayout.LabelField("Socket status:" + (receiver.SocketStarted ? " on" : " off"));
                EditorGUI.indentLevel++;
                toggleSuits = EditorGUILayout.Foldout(toggleSuits, "Live Smartsuits");
                if (toggleSuits)
                {
                    foreach (var suit in receiver.LiveSuits)
                    {
                        EditorGUILayout.LabelField(suit.Suitname + " " + suit.Fps + "fps");
                    }
                }

                EditorGUI.indentLevel--;
                GUILayout.EndVertical();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
            serializedObject.ApplyModifiedProperties();

        }
    }
}
/// @endcond