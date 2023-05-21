#if UNITY_EDITOR

using Rokoko.Helper;
using Rokoko.Inputs;
using UnityEditor;
using UnityEngine;

namespace Rokoko.RokokoEditor
{
    [CustomEditor(typeof(Character))]
    [CanEditMultipleObjects]
    public class CharacterEditor : TweakableEditor
    {
        private const int GroupSpace = 20;

        private SerializedProperty animatorProperty;
        private SerializedProperty profileNameProperty;
        private SerializedProperty faceProperty;

        private GameObject tPoseGuide;

        protected void OnEnable()
        {
            animatorProperty = serializedObject.FindProperty("animator");
            profileNameProperty = serializedObject.FindProperty("profileName");
            faceProperty = serializedObject.FindProperty("face");

            Character character = (Character)target;

            if (!Application.isPlaying)
                character.animator = character.gameObject.GetComponent<Animator>();

            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
        }

        private void OnDisable()
        {
        }

        private void HideTPoseGuide()
        {
        }

        private void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
        {
        }

        // Stops showing the script field
        protected override string[] GetInvisibleInDefaultInspector()
        {
            return new[] { "m_Script" };
        }

        public override void OnInspectorGUI()
        {
            Character character = (Character)target;
            serializedObject.Update();

            Undo.RecordObject(character, "Undo Character Changes");

            
            // Profile name
            {
                GUILayout.BeginVertical("HelpBox");

                EditorGUILayout.HelpBox("Profile name allows you to override any target from Studio", MessageType.Info);
                EditorGUILayout.PropertyField(profileNameProperty);

                GUILayout.EndVertical();

                GUILayout.Space(GroupSpace);

            }

            // Face
            {
                EditorGUILayout.LabelField("Character Face (Optional)", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(faceProperty);
                if (GUILayout.Button("Create"))
                {
                    if (character.gameObject.GetComponent<Face>() is Face face)
                    {
                        character.face = face;
                    }
                    else
                    {
                        character.face = Undo.AddComponent(character.gameObject, typeof(Face)) as Face;
                    }

                }
                if (GUILayout.Button("Self"))
                {
                    character.face = character.GetComponentInChildren<Face>();
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(10);
            }

            serializedObject.ApplyModifiedProperties();

            // Draw standard fields
            base.OnInspectorGUI();
        }
    }
}

#endif