using Rokoko.Core;
using Rokoko.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
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

        [HideInInspector] public BoneMappingEnum boneMapping;
        [HideInInspector] public Animator animator;
        [HideInInspector] public HumanBoneMapping customBoneMapping;

        [Header("Convert Space")]
        public Space positionSpace = Space.Self;
        public Space rotationSpace = Space.Self;

        public string profileName { get; private set; }

        [Header("Actor")]
        [SerializeField] protected Renderer meshRenderer = null;

        [Header("Actor Face (Optional)")]
        [SerializeField] protected Face face = null;
        [SerializeField] protected bool autoHideFaceWhenInactive = true;

        [Space(10)]
        public bool debug = false;

        protected Dictionary<HumanBodyBones, Transform> animatorHumanBones = new Dictionary<HumanBodyBones, Transform>();
        protected Material[] meshMaterials;

        #region Initialize

        protected virtual void Awake()
        {
            animator = this.GetComponent<Animator>();
            InitializeBodyBones();
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
        }

        #endregion

        #region Public Methods

        public virtual void UpdateActor(ActorFrame actorFrame)
        {
            profileName = actorFrame.name;
            this.gameObject.name = profileName;

            bool updateBody = actorFrame.meta.hasBody || actorFrame.meta.hasGloves;

            // Enable/Disable body renderer
            meshRenderer.enabled = updateBody;

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
                    UpdateBone(bone, boneFrame.Value, bone == HumanBodyBones.Hips);
            }
        }

        protected void UpdateBone(HumanBodyBones bone, ActorJointFrame jointFrame, bool updatePosition)
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
                    boneTransform.localPosition = jointFrame.position.ToVector3();
            }

            Quaternion worldRotation = jointFrame.rotation.ToQuaternion();
            if (rotationSpace == Space.World)
                boneTransform.rotation = worldRotation;
            else
                boneTransform.localRotation = Quaternion.Inverse(transform.parent.rotation) * worldRotation;
            //humanBones[bone].localRotation = jointFrame.rotation.ToQuaternion();
        }

        #endregion

    }
}