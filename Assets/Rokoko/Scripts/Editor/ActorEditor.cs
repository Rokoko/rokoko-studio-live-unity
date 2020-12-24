#if UNITY_EDITOR

using Rokoko.Helper;
using Rokoko.Inputs;
using UnityEditor;
using UnityEngine;

namespace Rokoko.RokokoEditor
{
    [CustomEditor(typeof(Actor))]
    [CanEditMultipleObjects]
    public class ActorEditor : TweakableEditor
    {
        private const string TPOSE_GUIDE_PREFAB_PATH = "TPoseGuide_Prefab";
        private const int GroupSpace = 20;

        private SerializedProperty boneMapping;
        private SerializedProperty animatorProperty;
        private SerializedProperty profileNameProperty;
        private SerializedProperty faceProperty;

        private GameObject tPoseGuide;

        protected void OnEnable()
        {
            boneMapping = serializedObject.FindProperty("boneMapping");
            animatorProperty = serializedObject.FindProperty("animator");
            profileNameProperty = serializedObject.FindProperty("profileName");
            faceProperty = serializedObject.FindProperty("face");

            Actor actor = (Actor)target;

            if (!Application.isPlaying)
                actor.animator = actor.gameObject.GetComponent<Animator>();

            if (actor.animator != null && !actor.isValidTpose)
                actor.CalculateTPose();

            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
        }

        private void OnDisable()
        {
            HideTPoseGuide();
        }

        private void HideTPoseGuide()
        {
            if (tPoseGuide != null)
            {
                RokokoHelper.Destroy(tPoseGuide);
                tPoseGuide = null;
            }
        }

        private void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
        {
            HideTPoseGuide();
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

            // TPose
            {
                GUILayout.BeginVertical("HelpBox");

                EditorGUILayout.HelpBox("T Pose reference is needed in order translate properly animation from Studio.", MessageType.Info);

                if (actor.animator != null)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("T Pose reference");

                        string textValue = (actor.characterTPose.Count == 0) ? "Not set" : $"Referece:{actor.name}";
                        GUILayout.TextField(textValue);

                        if (actor.characterTPose.Count == 0)
                            EditorGUILayout.HelpBox("You need to assign a reference T Pose.", MessageType.Error);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(10);

                    if (actor.characterTPose.Count > 0)
                    {
                        if (!actor.isValidTpose)
                        {
                            EditorGUILayout.HelpBox("Refernce T Pose seems wrong.\nRotate the character to match the TPose orientation guide and then \"Assign T Pose\" again.", MessageType.Error);

                            GUI.color = Color.white;
                        }
                    }

                    EditorGUILayout.BeginHorizontal();

                    if (tPoseGuide == null)
                    {
                        if (GUILayout.Button("Show T Pose Guide"))
                        {
                            tPoseGuide = GameObject.Instantiate(Resources.Load<GameObject>(TPOSE_GUIDE_PREFAB_PATH));
                            TPoseGuideGameComponent tposeComponent = tPoseGuide.GetComponent<TPoseGuideGameComponent>();
                            tposeComponent.followTarget = actor.transform;

                            float actorHeight = actor.GetActorHeight();
                            // Plane is x10 times bigger
                            // Make contour guide bigger
                            tposeComponent.transform.localScale = Vector3.one * actorHeight * 0.1f * 1.25f;
                            tposeComponent.followOffset = new Vector3(0, actorHeight / 2f, -0.5f);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Hide T Pose Guide"))
                        {
                            HideTPoseGuide();
                        }
                    }

                    if (GUILayout.Button("Assign T Pose"))
                    {
                        actor.CalculateTPose();
                    }

                    EditorGUILayout.EndHorizontal();

                    if (tPoseGuide != null)
                    {
                        EditorGUILayout.HelpBox("Rotate your character according to the help guide plane.\nNote: Position doesn't matter, you only need to match the actor to the silhouette", MessageType.Info);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Please select a valid Animator", MessageType.Error);
                }

                GUILayout.EndVertical();

                GUILayout.Space(GroupSpace);
            }

            // Profile name
            {
                GUILayout.BeginVertical("HelpBox");

                EditorGUILayout.HelpBox("Profile name allows you to override any target from Studio", MessageType.Info);
                EditorGUILayout.PropertyField(profileNameProperty);

                GUILayout.EndVertical();

                GUILayout.Space(GroupSpace);

            }

            // Bone Mapping
            {
                GUILayout.BeginVertical("HelpBox");

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
                        EditorGUILayout.HelpBox("Please select a valid Animator", MessageType.Error);
                    }
                    else if (!actor.animator.isHuman)
                    {
                        EditorGUILayout.HelpBox("The avatar you are using is not humanoid.\nPlease go in model inspector, under Rig tab and select AnimationType as Humanoid.", MessageType.Error);
                    }
                }
                else
                {
                    if (actor.GetComponent<HumanBoneMapping>() == null)
                    {
                        actor.customBoneMapping = Undo.AddComponent(actor.gameObject, typeof(HumanBoneMapping)) as HumanBoneMapping;
                    }
                    else if (actor.customBoneMapping == null)
                    {
                        actor.customBoneMapping = actor.GetComponent<HumanBoneMapping>();
                    }
                }

                GUILayout.EndVertical();

                GUILayout.Space(GroupSpace);
            }

            // Face
            {
                EditorGUILayout.LabelField("Actor Face (Optional)", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(faceProperty);
                if (GUILayout.Button("Create"))
                {
                    if (actor.gameObject.GetComponent<Face>() is Face face)
                    {
                        actor.face = face;
                    }
                    else
                    {
                        actor.face = Undo.AddComponent(actor.gameObject, typeof(Face)) as Face;
                    }

                }
                if (GUILayout.Button("Self"))
                {
                    actor.face = actor.GetComponentInChildren<Face>();
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