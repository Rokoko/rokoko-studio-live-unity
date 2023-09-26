using Rokoko.Core;
using Rokoko.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rokoko.Inputs
{
    public class Character : MonoBehaviour
    {
        [System.Serializable]
        public enum RotationSpace
        {
            Offset,
            World,
            Self
        }

        [HideInInspector] 
        public string profileName = "";

        [HideInInspector] public Animator animator;

        [Header("Convert Space")]
        [Tooltip("Convert Studio data to Unity position space")]
        public Space positionSpace = Space.World;
        [Tooltip("Convert Studio data to Unity rotation space")]
        public RotationSpace rotationSpace = RotationSpace.World;

        [Tooltip("A rotation pre rotation applied")]
        public Vector3 PreRotation = new Vector3(0.0f, 0.0f, 0.0f);

        [Tooltip("A rotation post rotation applied")]
        public Vector3 PostRotation = new Vector3(0.0f, 0.0f, 0.0f);

        [Space(10)]
        [Tooltip("Calculate Model's height comparing to Actor's and position the Hips accordingly.\nGreat tool to align with the floor")]
        public bool adjustHipHeightBasedOnStudioActor = false;

        public string hipsName = "pelvis";

        [HideInInspector] public Face face = null;

        [Header("Log extra info")]
        public bool debug = false;


        private Dictionary<string, Transform> _skeletonJoints = new Dictionary<string, Transform>();

        #region Initialize

        protected virtual void Awake()
        {
            if(animator == null)
            {
                Debug.LogError($"Character {this.name} isn't configured", this.transform);
                //return;
            }

            InitializeSkeletonJoints();
        }

        private void InitializeSkeletonJoints()
        {
            _skeletonJoints.Clear();
            Transform[] transforms = GetComponentsInChildren<Transform>();
            for (int i=0; i<transforms.Length; ++i)
            {
                if (transforms[i].GetComponentInChildren<SkinnedMeshRenderer>() != null)
                    continue;

                _skeletonJoints.Add(transforms[i].name, transforms[i]);
            }
        }

        /// <summary>
        /// Register Actor override in StudioManager.
        /// </summary>
        private void Start()
        {
            //if (animator != null && !animator.isHuman) return;

            if (!string.IsNullOrEmpty(profileName))
                StudioManager.AddCharacterOverride(this);
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Update Skeleton and Face data based on ActorFrame.
        /// </summary>
        public virtual void UpdateCharacter(CharacterFrame frame)
        {
            //if (animator == null || !animator.isHuman) return;

            profileName = frame.name;

            // Update skeleton from data
            UpdateSkeleton(frame);

            if (frame.blendshapes != null && frame.blendshapes.names != null && frame.blendshapes.names.Length > 0)
            {
                face?.UpdateFace(frame.blendshapes.names, frame.blendshapes.values);    
            }
            else if (debug)
            {
                Debug.LogError($"Character {this.name} face has no blendshapes");
            }
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
        protected void UpdateSkeleton(CharacterFrame frame)
        {
            for (int i=0; i<frame.joints.Length; ++i)
            {
                if (_skeletonJoints.TryGetValue(frame.joints[i].name, out Transform transform))
                {
                    bool shouldUpdatePosition = transform.name == hipsName;

                    Quaternion worldRotation = frame.joints[i].rotation.ToQuaternion();
                    Vector3 worldPosition = frame.joints[i].position.ToVector3();

                    UpdateBone(transform, worldPosition, worldRotation, shouldUpdatePosition, positionSpace, rotationSpace);       
                }
            }
        }

        /// <summary>
        /// Update Human bone.
        /// </summary>
        protected void UpdateBone(Transform boneTransform, Vector3 worldPosition, Quaternion worldRotation, bool updatePosition, Space positionSpace, RotationSpace rotationSpace)
        {
            // Update position
            if (updatePosition)
            {
                if (positionSpace == Space.World || boneTransform.parent == null)
                {
                    boneTransform.position = worldPosition;
                }
                else
                {
                    boneTransform.position = boneTransform.parent.rotation * worldPosition + boneTransform.parent.position;
                }
            }

            // Update Rotation
            if (rotationSpace == RotationSpace.World)
            {
                boneTransform.rotation = Quaternion.Euler(PreRotation.x, PreRotation.y, PreRotation.z) * worldRotation * Quaternion.Euler(PostRotation.x, PostRotation.y, PostRotation.z);
            }
            else if (rotationSpace == RotationSpace.Self)
            {
                if (transform.parent != null)
                    boneTransform.rotation = this.transform.parent.rotation * worldRotation;
                else
                    boneTransform.rotation = worldRotation;
            }
        }

        #endregion

    }
}