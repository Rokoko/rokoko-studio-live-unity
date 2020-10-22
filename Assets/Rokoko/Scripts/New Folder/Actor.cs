using Rokoko.Serializers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rokoko
{
    [RequireComponent(typeof(Animator))]
    public class Actor : MonoBehaviour
    {
        public string actorName { get; private set; }

        [SerializeField] protected Renderer meshRenderer = null;
        [SerializeField] protected Face face = null;

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
            actorName = actorFrame.name;
            this.gameObject.name = actorName;

            bool updateBody = actorFrame.meta.hasBody || actorFrame.meta.hasGloves;

            // Enable/Disable body renderer
            meshRenderer.enabled = updateBody;

            // Update skeleton from data
            if (updateBody)
                UpdateSkeleton(actorFrame.body);

            // Enable/Disable face renderer
            face.gameObject.SetActive(actorFrame.meta.hasFace);

            // Update face from data
            if (actorFrame.meta.hasFace)
                face.UpdateFace(actorFrame.face);
        }

        public virtual void CreateIdle(string actorName)
        {
            this.actorName = actorName;
            face.gameObject.SetActive(false);
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
                    UpdateBone(bone, boneFrame.Value);
            }
        }

        protected void UpdateBone(HumanBodyBones bone, ActorJointFrame jointFrame)
        {
            humanBones[bone].position = jointFrame.position.ToVector3();
            humanBones[bone].rotation = jointFrame.rotation.ToQuaternion();
        }

        #endregion

    }
}