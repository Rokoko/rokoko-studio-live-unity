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

        //[Header("Actor Face (Optional)")]
        [HideInInspector] public Face face = null;
        [HideInInspector] public bool autoHideFaceWhenInactive = true;

        private Dictionary<HumanBodyBones, Quaternion> offsets = new Dictionary<HumanBodyBones, Quaternion>();

        [Header("Log extra info")]
        public bool debug = false;

        protected Dictionary<HumanBodyBones, Transform> animatorHumanBones = new Dictionary<HumanBodyBones, Transform>();
        protected Material[] meshMaterials;

        #region Initialize

        protected virtual void Awake()
        {
            animator = this.GetComponent<Animator>();
            InitializeBodyBones();
        }

        private void Start()
        {
            if (!string.IsNullOrEmpty(profileName))
                StudioManager.AddActorOverride(this);
        }

        // Cache the bone transforms from Animator
        protected void InitializeBodyBones()
        {
            if (animator == null) return;

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

        public virtual void UpdateActor(ActorFrame actorFrame)
        {
            profileName = actorFrame.name;
            this.gameObject.name = profileName;

            bool updateBody = actorFrame.meta.hasBody || actorFrame.meta.hasGloves;

            // Update skeleton from data
            if (updateBody)
                UpdateSkeleton(actorFrame.body);

            // Enable/Disable face renderer
            if (autoHideFaceWhenInactive)
                face?.gameObject.SetActive(actorFrame.meta.hasFace);

            // Update face from data
            if (actorFrame.meta.hasFace)
                face?.UpdateFace(actorFrame.face);
        }

        public virtual void CreateIdle(string actorName)
        {
            this.profileName = actorName;

            if (autoHideFaceWhenInactive)
                face?.gameObject.SetActive(false);
        }

        #endregion

        #region Internal Logic

        protected void UpdateSkeleton(BodyFrame bodyFrame)
        {
            foreach (HumanBodyBones bone in RokokoHelper.HumanBodyBonesArray)
            {
                if (bone == HumanBodyBones.LastBone) break;
                ActorJointFrame? boneFrame = bodyFrame.GetBoneFrame(bone);
                if (boneFrame != null)
                    UpdateBone(bone, boneFrame.Value, bone == HumanBodyBones.Hips, positionSpace, rotationSpace);
            }

            //THIS IS THE LERP FIX THAT OVERRIDES FIRMWARE LERP FOR A1 SENSOR
            //rotations[(int)HumanBodyBones.Spine] = Quaternion.Lerp(rotations[(int)HumanBodyBones.Hips], rotations[(int)HumanBodyBones.Chest], 0.5f);
        }

        protected void UpdateBone(HumanBodyBones bone, ActorJointFrame jointFrame, bool updatePosition, Space positionSpace, RotationSpace rotationSpace)
        {
            Transform boneTransform = null;
            if (boneMapping == BoneMappingEnum.Animator)
                boneTransform = animatorHumanBones[bone];
            else
                boneTransform = customBoneMapping.customBodyBones[(int)bone];

            if (boneTransform == null)
            {
                if (debug)
                    Debug.LogWarning($"Couldn't find Transform for bone:{bone} in {boneMapping}Mapping component", this.transform);
                return;
            }

            if (updatePosition)
            {
                if (positionSpace == Space.World)
                    boneTransform.position = jointFrame.position.ToVector3();
                else
                {
                    if (transform.parent != null)
                        boneTransform.localPosition = jointFrame.position.ToVector3();
                    else
                        boneTransform.position = jointFrame.position.ToVector3();
                }
            }

            Quaternion worldRotation = jointFrame.rotation.ToQuaternion();
            if (rotationSpace == RotationSpace.World)
                boneTransform.rotation = worldRotation;
            else if (rotationSpace == RotationSpace.Self)
            {
                boneTransform.rotation = this.transform.parent.rotation * worldRotation;
            }
            else
            {
                boneTransform.rotation = this.transform.rotation * jointFrame.rotation.ToQuaternion() * offsets[bone];
            }
        }

        #endregion

        private static Dictionary<HumanBodyBones, Quaternion> GetRotationOffsets(Dictionary<HumanBodyBones, Transform> humanoidBones)
        {
            Dictionary<HumanBodyBones, Quaternion> offsets = new Dictionary<HumanBodyBones, Quaternion>();
            foreach (HumanBodyBones bone in RokokoHelper.HumanBodyBonesArray)
            {
                Quaternion rotation = Quaternion.identity;
                if (humanoidBones[bone] != null)
                    rotation = Quaternion.Inverse(SmartsuitTPose[bone]) * humanoidBones[bone].rotation;

                offsets.Add(bone, rotation);
            }
            return offsets;
        }

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