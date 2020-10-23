using Rokoko.Core;
using Rokoko.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rokoko.Inputs
{
    [RequireComponent(typeof(Animator))]
    public class Actor : MonoBehaviour
    {
        public Space positionSpace = Space.Self;
        public Space rotationSpace = Space.Self;

        public string profileName { get; private set; }

        [Header("Actor")]
        [SerializeField] protected Renderer meshRenderer = null;

        [Header("Actor Face (Optional)")]
        [SerializeField] protected Face face = null;
        [SerializeField] protected bool autoHideFaceWhenInactive = true;

        protected Dictionary<HumanBodyBones, Transform> humanBones = new Dictionary<HumanBodyBones, Transform>();
        protected Animator animator;
        protected Material[] meshMaterials;

        #region Initialize

        protected virtual void Awake()
        {
            animator = this.GetComponent<Animator>();
            InitializeBodyBones();
        }

        // Cache the bone transforms
        private void InitializeBodyBones()
        {
            foreach (HumanBodyBones bone in System.Enum.GetValues(typeof(HumanBodyBones)))
            {
                if (bone == HumanBodyBones.LastBone) break;
                humanBones.Add(bone, animator.GetBoneTransform(bone));
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
            foreach (HumanBodyBones bone in System.Enum.GetValues(typeof(HumanBodyBones)))
            {
                if (bone == HumanBodyBones.LastBone) break;
                ActorJointFrame? boneFrame = bodyFrame.GetBoneFrame(bone);
                if (boneFrame != null)
                    UpdateBone(bone, boneFrame.Value, bone == HumanBodyBones.Hips);
            }
        }

        protected void UpdateBone(HumanBodyBones bone, ActorJointFrame jointFrame, bool updatePosition)
        {
            if (updatePosition)
            {
                if (positionSpace == Space.World)
                    humanBones[bone].position = jointFrame.position.ToVector3();
                else
                    humanBones[bone].localPosition = jointFrame.position.ToVector3();

            }

            Quaternion worldRotation = jointFrame.rotation.ToQuaternion();
            if (rotationSpace == Space.World)
                humanBones[bone].rotation = worldRotation;
            else
                humanBones[bone].localRotation = Quaternion.Inverse(transform.parent.rotation) * worldRotation;
            //humanBones[bone].localRotation = jointFrame.rotation.ToQuaternion();
        }

        #endregion

    }
}