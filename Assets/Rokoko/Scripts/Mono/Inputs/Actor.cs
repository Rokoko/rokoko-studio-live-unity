using Rokoko.Core;
using Rokoko.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rokoko.Inputs
{
    public class Actor : MonoBehaviour
    {
        [System.Serializable]
        public enum BoneMappingEnum
        {
            Animator,
            Custom
        }

        [System.Serializable]
        public enum RotationSpace
        {
            Offset,
            World,
            Self
        }

        [HideInInspector] public string profileName = "DemoProfile";

        [HideInInspector] public BoneMappingEnum boneMapping;
        [HideInInspector] public Animator animator;
        [HideInInspector] public HumanBoneMapping customBoneMapping;

        [Header("Convert Space")]
        [Tooltip("Convert Studio data to Unity position space")]
        public Space positionSpace = Space.Self;
        [Tooltip("Convert Studio data to Unity rotation space")]
        public RotationSpace rotationSpace = RotationSpace.Offset;

        [Space(10)]
        [Tooltip("Calculate Model's height comparing to Actor's and position the Hips accordingly.\nGreat tool to align with the floor")]
        public bool adjustHipHeightBasedOnStudioActor = true;

        [HideInInspector] public Face face = null;

        [Header("Log extra info")]
        public bool debug = false;

        protected Dictionary<HumanBodyBones, Transform> animatorHumanBones = new Dictionary<HumanBodyBones, Transform>();
        private Dictionary<HumanBodyBones, Quaternion> offsets = new Dictionary<HumanBodyBones, Quaternion>();

        private float hipHeight = 0;

        #region Initialize

        protected virtual void Awake()
        {
            if (!animator.isHuman)
            {
                Debug.LogError("Model is not marked as Humanoid. Please go in model inspector, under Rig tab and select AnimationType as Humanoid.");
                return;
            }

            InitializeBodyBones();

            // Get the Hip height independent of parent transformations
            hipHeight = animatorHumanBones[HumanBodyBones.Hips].parent.InverseTransformVector(animatorHumanBones[HumanBodyBones.Hips].localPosition).y;
            hipHeight = Mathf.Abs(hipHeight);
        }

        /// <summary>
        /// Register Actor override in StudioManager.
        /// </summary>
        private void Start()
        {
            if (!animator.isHuman) return;

            if (!string.IsNullOrEmpty(profileName))
                StudioManager.AddActorOverride(this);
        }

        /// <summary>
        /// Cache the bone transforms from Animator.
        /// </summary>
        protected void InitializeBodyBones()
        {
            if (animator == null || !animator.isHuman) return;

            foreach (HumanBodyBones bone in RokokoHelper.HumanBodyBonesArray)
            {
                if (bone == HumanBodyBones.LastBone) break;
                animatorHumanBones.Add(bone, animator.GetBoneTransform(bone));
            }

            // Calculate offsets based on Smartsuit T pose
            offsets = GetRotationOffsets(animatorHumanBones);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update Skeleton and Face data based on ActorFrame.
        /// </summary>
        public virtual void UpdateActor(ActorFrame actorFrame)
        {
            if (animator == null || !animator.isHuman) return;

            profileName = actorFrame.name;

            bool updateBody = actorFrame.meta.hasBody || actorFrame.meta.hasGloves;

            // Update skeleton from data
            if (updateBody)
                UpdateSkeleton(actorFrame);

            // Update face from data
            if (actorFrame.meta.hasFace)
                face?.UpdateFace(actorFrame.face);
        }

        /// <summary>
        /// Create Idle/Default Actor.
        /// </summary>
        public virtual void CreateIdle(string actorName)
        {
            this.profileName = actorName;
        }

        #endregion

        #region Internal Logic

        /// <summary>
        /// Update Humanoid Skeleton based on BodyData.
        /// </summary>
        protected void UpdateSkeleton(ActorFrame actorFrame)
        {
            foreach (HumanBodyBones bone in RokokoHelper.HumanBodyBonesArray)
            {
                if (bone == HumanBodyBones.LastBone) break;
                ActorJointFrame? boneFrame = actorFrame.body.GetBoneFrame(bone);
                if (boneFrame != null)
                {
                    bool shouldUpdatePosition = bone == HumanBodyBones.Hips;

                    Quaternion worldRotation = boneFrame.Value.rotation.ToQuaternion();
                    Vector3 worldPosition = boneFrame.Value.position.ToVector3();

                    // Offset Hip bone
                    if (shouldUpdatePosition && adjustHipHeightBasedOnStudioActor)
                        worldPosition = new Vector3(worldPosition.x, worldPosition.y - (actorFrame.dimensions.hipHeight - hipHeight), worldPosition.z);

                    UpdateBone(bone, worldPosition, worldRotation, shouldUpdatePosition, positionSpace, rotationSpace);
                }
            }
        }

        /// <summary>
        /// Update Human bone.
        /// </summary>
        protected void UpdateBone(HumanBodyBones bone, Vector3 worldPosition, Quaternion worldRotation, bool updatePosition, Space positionSpace, RotationSpace rotationSpace)
        {
            // Find Humanoid bone
            Transform boneTransform;
            if (boneMapping == BoneMappingEnum.Animator)
                boneTransform = animatorHumanBones[bone];
            else
                boneTransform = customBoneMapping.customBodyBones[(int)bone];

            // Check if bone is valid
            if (boneTransform == null)
            {
                if (debug)
                    Debug.LogWarning($"Couldn't find Transform for bone:{bone} in {boneMapping}Mapping component", this.transform);
                return;
            }

            // Update position
            if (updatePosition)
            {
                if (positionSpace == Space.World || transform.parent == null)
                {
                    boneTransform.localPosition = worldPosition;
                }
                else
                {
                    boneTransform.localPosition = worldPosition;
                }
            }

            // Update Rotation
            if (rotationSpace == RotationSpace.World)
            {
                boneTransform.rotation = worldRotation;
            }
            else if (rotationSpace == RotationSpace.Self)
            {
                if (transform.parent != null)
                    boneTransform.rotation = this.transform.parent.rotation * worldRotation;
                else
                    boneTransform.rotation = worldRotation;
            }
            else
            {
                boneTransform.rotation = this.transform.rotation * worldRotation * offsets[bone];
            }
        }

        #endregion

        /// <summary>
        /// Get the rotational difference between 2 humanoid T poses.
        /// </summary>
        private static Dictionary<HumanBodyBones, Quaternion> GetRotationOffsets(Dictionary<HumanBodyBones, Transform> humanoidBones)
        {
            Dictionary<HumanBodyBones, Quaternion> offsets = new Dictionary<HumanBodyBones, Quaternion>();
            foreach (HumanBodyBones bone in RokokoHelper.HumanBodyBonesArray)
            {
                Quaternion rotation = Quaternion.identity;
                if (humanoidBones[bone] != null)
                {
                    // Subtract Root orientation from bone
                    Quaternion boneTransform = Quaternion.Inverse(humanoidBones[HumanBodyBones.Hips].parent.rotation) * humanoidBones[bone].rotation;
                    // Subtract from SmartSuit T Pose
                    rotation = Quaternion.Inverse(SmartsuitTPose[bone]) * boneTransform;
                }

                offsets.Add(bone, rotation);
            }
            return offsets;
        }

        /// <summary>
        /// Get Smartsuit T pose data
        /// </summary>
        private static Dictionary<HumanBodyBones, Quaternion> SmartsuitTPose = new Dictionary<HumanBodyBones, Quaternion>() {
            {HumanBodyBones.Hips, new Quaternion(0.000f, 0.000f, 0.000f, 1.000f)},
            {HumanBodyBones.LeftUpperLeg, new Quaternion(0.000f, 0.707f, 0.000f, 0.707f)},
            {HumanBodyBones.RightUpperLeg, new Quaternion(0.000f, -0.707f, 0.000f, 0.707f)},
            {HumanBodyBones.LeftLowerLeg, new Quaternion(0.000f, 0.707f, 0.000f, 0.707f)},
            {HumanBodyBones.RightLowerLeg, new Quaternion(0.000f, -0.707f, 0.000f, 0.707f)},
            {HumanBodyBones.LeftFoot, new Quaternion(0.000f, 0.707f, -0.707f, 0.000f)},
            {HumanBodyBones.RightFoot, new Quaternion(0.000f, -0.707f, 0.707f, 0.000f)},
            {HumanBodyBones.Spine, new Quaternion(0.000f, 0.000f, 1.000f, 0.000f)},
            {HumanBodyBones.Chest, new Quaternion(0.000f, 0.000f, 1.000f, 0.000f)},
            {HumanBodyBones.Neck, new Quaternion(0.000f, 0.000f, 1.000f, 0.000f)},
            {HumanBodyBones.Head, new Quaternion(0.000f, 0.000f, 1.000f, 0.000f)},
            {HumanBodyBones.LeftShoulder, new Quaternion(0.000f, 0.000f, 0.707f, -0.707f)},
            {HumanBodyBones.RightShoulder, new Quaternion(0.000f, 0.000f, 0.707f, 0.707f)},
            {HumanBodyBones.LeftUpperArm, new Quaternion(-0.500f, -0.500f, 0.500f, -0.500f)},
            {HumanBodyBones.RightUpperArm, new Quaternion(0.500f, -0.500f, 0.500f, 0.500f)},
            {HumanBodyBones.LeftLowerArm, new Quaternion(-0.500f, -0.500f, 0.500f, -0.500f)},
            {HumanBodyBones.RightLowerArm, new Quaternion(0.500f, -0.500f, 0.500f, 0.500f)},
            {HumanBodyBones.LeftHand, new Quaternion(-0.500f, -0.500f, 0.500f, -0.500f)},
            {HumanBodyBones.RightHand, new Quaternion(0.500f, -0.500f, 0.500f, 0.500f)},
            {HumanBodyBones.LeftToes, new Quaternion(0.000f, 0.707f, -0.707f, 0.000f)},
            {HumanBodyBones.RightToes, new Quaternion(0.000f, -0.707f, 0.707f, 0.000f)},
            {HumanBodyBones.LeftEye, new Quaternion(0.000f, 0.000f, 0.000f, 0.000f)},
            {HumanBodyBones.RightEye, new Quaternion(0.000f, 0.000f, 0.000f, 0.000f)},
            {HumanBodyBones.Jaw, new Quaternion(0.000f, 0.000f, 0.000f, 0.000f)},
            {HumanBodyBones.LeftThumbProximal, new Quaternion(-0.561f, -0.701f, 0.430f, -0.092f)},
            {HumanBodyBones.LeftThumbIntermediate, new Quaternion(-0.653f, -0.653f, 0.271f, -0.271f)},
            {HumanBodyBones.LeftThumbDistal, new Quaternion(-0.653f, -0.653f, 0.271f, -0.271f)},
            {HumanBodyBones.LeftIndexProximal, new Quaternion(-0.500f, -0.500f, 0.500f, -0.500f)},
            {HumanBodyBones.LeftIndexIntermediate, new Quaternion(-0.500f, -0.500f, 0.500f, -0.500f)},
            {HumanBodyBones.LeftIndexDistal, new Quaternion(-0.500f, -0.500f, 0.500f, -0.500f)},
            {HumanBodyBones.LeftMiddleProximal, new Quaternion(-0.500f, -0.500f, 0.500f, -0.500f)},
            {HumanBodyBones.LeftMiddleIntermediate, new Quaternion(-0.500f, -0.500f, 0.500f, -0.500f)},
            {HumanBodyBones.LeftMiddleDistal, new Quaternion(-0.500f, -0.500f, 0.500f, -0.500f)},
            {HumanBodyBones.LeftRingProximal, new Quaternion(-0.500f, -0.500f, 0.500f, -0.500f)},
            {HumanBodyBones.LeftRingIntermediate, new Quaternion(-0.500f, -0.500f, 0.500f, -0.500f)},
            {HumanBodyBones.LeftRingDistal, new Quaternion(-0.500f, -0.500f, 0.500f, -0.500f)},
            {HumanBodyBones.LeftLittleProximal, new Quaternion(-0.500f, -0.500f, 0.500f, -0.500f)},
            {HumanBodyBones.LeftLittleIntermediate, new Quaternion(-0.500f, -0.500f, 0.500f, -0.500f)},
            {HumanBodyBones.LeftLittleDistal, new Quaternion(-0.500f, -0.500f, 0.500f, -0.500f)},
            {HumanBodyBones.RightThumbProximal, new Quaternion(0.561f, -0.701f, 0.430f, 0.092f)},
            {HumanBodyBones.RightThumbIntermediate, new Quaternion(0.653f, -0.653f, 0.271f, 0.271f)},
            {HumanBodyBones.RightThumbDistal, new Quaternion(0.653f, -0.653f, 0.271f, 0.271f)},
            {HumanBodyBones.RightIndexProximal, new Quaternion(0.500f, -0.500f, 0.500f, 0.500f)},
            {HumanBodyBones.RightIndexIntermediate, new Quaternion(0.500f, -0.500f, 0.500f, 0.500f)},
            {HumanBodyBones.RightIndexDistal, new Quaternion(0.500f, -0.500f, 0.500f, 0.500f)},
            {HumanBodyBones.RightMiddleProximal, new Quaternion(0.500f, -0.500f, 0.500f, 0.500f)},
            {HumanBodyBones.RightMiddleIntermediate, new Quaternion(0.500f, -0.500f, 0.500f, 0.500f)},
            {HumanBodyBones.RightMiddleDistal, new Quaternion(0.500f, -0.500f, 0.500f, 0.500f)},
            {HumanBodyBones.RightRingProximal, new Quaternion(0.500f, -0.500f, 0.500f, 0.500f)},
            {HumanBodyBones.RightRingIntermediate, new Quaternion(0.500f, -0.500f, 0.500f, 0.500f)},
            {HumanBodyBones.RightRingDistal, new Quaternion(0.500f, -0.500f, 0.500f, 0.500f)},
            {HumanBodyBones.RightLittleProximal, new Quaternion(0.500f, -0.500f, 0.500f, 0.500f)},
            {HumanBodyBones.RightLittleIntermediate, new Quaternion(0.500f, -0.500f, 0.500f, 0.500f)},
            {HumanBodyBones.RightLittleDistal, new Quaternion(0.500f, -0.500f, 0.500f, 0.500f)},
            {HumanBodyBones.UpperChest, new Quaternion(0.000f, 0.000f, 1.000f, 0.000f)}
        };
    }
}