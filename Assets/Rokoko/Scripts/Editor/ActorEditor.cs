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
            actor.animator = actor.gameObject.GetComponent<Animator>();

            if (!actor.isValidTpose)
                actor.CalculateTPose();
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

                if (actor.characterTPose.Count == 0)
                {
                    EditorGUILayout.HelpBox("You need to assign a reference T Pose.", MessageType.Error);
                }
                else
                {
                    if (!actor.isValidTpose)
                    {
                        EditorGUILayout.HelpBox("Refernce T Pose seems wrong.\nRotate the character to match the TPose guide orientation and then \"Assign T Pose\" again.", MessageType.Error);
                    }
                }

                EditorGUILayout.BeginHorizontal();

                if (tPoseGuide == null)
                {
                    if (GUILayout.Button("Show T Pose Guide"))
                    {
                        tPoseGuide = GameObject.Instantiate(Resources.Load<GameObject>(TPOSE_GUIDE_PREFAB_PATH));
                        TPoseGuideGameComponent tposeComponent = tPoseGuide.GetComponent<TPoseGuideGameComponent>();
                        tposeComponent.followTarget = actor.gameObject;
                        
                        float actorHeight = actor.GetComponentInChildren<SkinnedMeshRenderer>()?.sharedMesh.bounds.size.y ?? 1.8f;
                        // Plane is x10 times bigger
                        // Make contour buide bigger
                        tposeComponent.transform.localScale = Vector3.one * actorHeight * 0.1f * 1.25f;
                        tposeComponent.followOffset = new Vector3(0, actorHeight / 2f, -0.5f);
                        Debug.Log(actorHeight);
                        Debug.Log(tposeComponent.transform.localScale);
                    }
                }
                else
                {
                    if (GUILayout.Button("Hide T Pose Guide"))
                    {
                        HideTPoseGuide();
                    }
                }

                if (actor.animator != null)
                {
                    if (GUILayout.Button("Assign T Pose"))
                    {
                        actor.CalculateTPose();
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("In order to assign a T Pose you first need to assign an Animator or add a HumanBoneMappingEditor", MessageType.Error);
                }

                EditorGUILayout.EndHorizontal();
                
                GUILayout.EndVertical();

                GUILayout.Space(GroupSpace);
            }

            // Profile name
            {
                EditorGUILayout.HelpBox("Profile name allows you to override any target from Studio", MessageType.Info);
                EditorGUILayout.PropertyField(profileNameProperty);

                GUILayout.Space(GroupSpace);
            }

            // Bone Mapping
            {
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