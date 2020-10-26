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

        [HideInInspector] public BoneMappingEnum boneMapping;
        [HideInInspector] public Animator animator;
        [HideInInspector] public HumanBoneMapping customBoneMapping;

        [Header("Convert Space")]
        public Space positionSpace = Space.Self;
        public Space rotationSpace = Space.Self;

        public string profileName;

        [Header("Actor Face (Optional)")]
        [SerializeField] protected Face face = null;
        [SerializeField] protected bool autoHideFaceWhenInactive = true;


        [Header("WIP")]
        [SerializeField] protected Animator newtonAnimator;
        public bool useOffset = true;
        private BodyPose tPose;
        private Quaternion[] offsets = new Quaternion[0];

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

        private void Start()
        {
            if (!string.IsNullOrEmpty(profileName))
                ActorOverrides.AddActorOverride(this);
        }

        // Cache the bone transforms from Animator
        protected void InitializeBodyBones()
        {
            if (animator == null) return;

            foreach (HumanBodyBones bone in RokokoHelper.HumanBodyBonesArray)
            {
                if (bone == HumanBodyBones.LastBone) break;
                animatorHumanBones.Add(bone, animator.GetBoneTransform(bone));
                //if (newtonAnimator.GetBoneTransform(bone) is Transform joint)
                //    newtonTPose.Add(bone, joint.rotation);
                //if (animator.GetBoneTransform(bone) is Transform joint2)
                //    selfOffsets.Add(bone, joint2.rotation);
            }

            if (newtonAnimator != null)
            {
                tPose = new BodyPose();
                tPose.forward = this.transform.forward;
                tPose.store(animatorHumanBones.Values.ToArray());

                BodyPose newtonPose = new BodyPose();
                newtonPose.forward = newtonAnimator.transform.forward;
                Transform[] newtonBones = new Transform[(int)HumanBodyBones.LastBone];
                for (int i = 0; i < newtonBones.Length; i++)
                {
                    newtonBones[i] = newtonAnimator.GetBoneTransform((HumanBodyBones)i);
                    //if (newtonAnimator.GetBoneTransform(bone) is Transform joint)
                    //    newtonTPose.Add(bone, joint.rotation);
                    //if (animator.GetBoneTransform(bone) is Transform joint2)
                    //    selfOffsets.Add(bone, joint2.rotation);
                }
                newtonPose.store(newtonBones);
                offsets = tPose.ExtractRotationOffsets(newtonPose);
            }
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
                    UpdateBone(bone, boneFrame.Value, bone == HumanBodyBones.Hips, positionSpace, Space.World);
            }

            if (rotationSpace == Space.Self)
            {
                UpdateBone(HumanBodyBones.Hips, bodyFrame.GetBoneFrame(HumanBodyBones.Hips).Value, false, positionSpace, Space.Self);
            }

            //THIS IS THE LERP FIX THAT OVERRIDES FIRMWARE LERP FOR A1 SENSOR
            //rotations[(int)HumanBodyBones.Spine] = Quaternion.Lerp(rotations[(int)HumanBodyBones.Hips], rotations[(int)HumanBodyBones.Chest], 0.5f);
        }

        protected void UpdateBone(HumanBodyBones bone, ActorJointFrame jointFrame, bool updatePosition, Space positionSpace, Space rotationSpace)
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
            {
                boneTransform.rotation = this.transform.parent.rotation * worldRotation;
            }

            if (useOffset && offsets.Length > (int)bone && bone != HumanBodyBones.Hips)
            {
                boneTransform.Rotate(worldRotation.eulerAngles, Space.Self);
                boneTransform.rotation = this.transform.rotation * jointFrame.rotation.ToQuaternion() * offsets[(int)bone];
            }
        }

        //private Dictionary<HumanBodyBones, Quaternion> newtonTPose = new Dictionary<HumanBodyBones, Quaternion>();
        //private Dictionary<HumanBodyBones, Quaternion> selfOffsets = new Dictionary<HumanBodyBones, Quaternion>();

        #endregion

    }



    [System.Serializable]
    internal struct BodyPose
    {
        public string name;
        public bool stored;
        public Vector3 hip;
        public Vector3 forward;
        public Quaternion[] rotation;
        public int Length
        {
            get
            {
                if (rotation != null) return rotation.Length;
                else return 0;
            }
            set { rotation = new Quaternion[value]; }
        }
        public Vector3 left
        {
            get
            {
                return Vector3.Cross(forward, Vector3.up);
            }
        }
        public void store(Transform[] bones)
        {
            Length = bones.Length;
            hip = bones[0].localPosition;
            if (bones[0].parent == null) forward = bones[0].forward;
            else forward = bones[0].parent.forward;
            forward.y = 0; forward.Normalize();
            //Debug.DrawRay(bones[0].position, forward, Color.blue, 5);
            //Debug.DrawRay(bones[0].position, left, Color.red, 3);
            for (int i = 0; i < Length; i++)
            {
                if (bones[i] != null)
                    rotation[i] = bones[i].rotation;
            }
            stored = true;
        }
        public Vector3 estimateForwardFrom(Transform[] bones)
        {
            if (stored && bones.Length == Length)
            {
                Vector3 ff = Vector3.zero;
                int c = 0;
                for (int i = 0; i < Length; i++)
                {
                    if (bones[i] != null)
                    {
                        ff += (bones[i].rotation * Quaternion.Inverse(rotation[i])) * forward;
                        c++;
                    }
                }
                return ff / c;
            }
            else
                return forward;
        }
        public void applyPoseTo(Transform[] bones)
        {
            if (Length == bones.Length)
            {
                bones[0].localPosition = hip;
                for (int i = 0; i < Length; i++) if (bones[i] != null) bones[i].rotation = rotation[i];
            }
        }

        public Quaternion[] ExtractRotationOffsets(BodyPose other)
        {
            Quaternion[] q = new Quaternion[Mathf.Min(other.rotation.Length, this.rotation.Length)];
            //forward has to be set correctly
            Quaternion otherforward = Quaternion.Inverse(Quaternion.LookRotation(other.forward));
            Quaternion thisforward = Quaternion.Inverse(Quaternion.LookRotation(forward));
            for (int i = 0; i < q.Length; i++)
            {
                q[i] = Quaternion.Inverse(otherforward * other.rotation[i]) * thisforward * this.rotation[i];
            }
            return q;
        }

        internal static BodyPose SmartsuitTpose()
        {
            BodyPose tpose = new BodyPose();
            tpose.Length = 19;
            var spine = Quaternion.Euler(0, 0, 180);
            var leftArm = Quaternion.Euler(90, 0, -90);
            var rightArm = Quaternion.Euler(90, 0, 90);
            var leftLeg = Quaternion.Euler(0, 90, 0);
            var rightLeg = Quaternion.Euler(0, -90, 0);
            var foot = Quaternion.Euler(90, 180, 0);
            tpose.rotation[(int)HumanBodyBones.LeftShoulder] = Quaternion.Euler(0, 0, -90);
            tpose.rotation[(int)HumanBodyBones.RightShoulder] = Quaternion.Euler(0, 0, 90);
            tpose.rotation[(int)HumanBodyBones.Hips] = spine;
            tpose.rotation[(int)HumanBodyBones.Spine] = spine;
            tpose.rotation[(int)HumanBodyBones.Chest] = spine;
            tpose.rotation[(int)HumanBodyBones.Neck] = spine;
            tpose.rotation[(int)HumanBodyBones.Head] = spine;
            tpose.rotation[(int)HumanBodyBones.LeftUpperArm] = leftArm;
            tpose.rotation[(int)HumanBodyBones.LeftLowerArm] = leftArm;
            tpose.rotation[(int)HumanBodyBones.LeftHand] = leftArm;
            tpose.rotation[(int)HumanBodyBones.RightUpperArm] = rightArm;
            tpose.rotation[(int)HumanBodyBones.RightLowerArm] = rightArm;
            tpose.rotation[(int)HumanBodyBones.RightHand] = rightArm;
            tpose.rotation[(int)HumanBodyBones.LeftUpperLeg] = leftLeg;
            tpose.rotation[(int)HumanBodyBones.LeftLowerLeg] = leftLeg;
            tpose.rotation[(int)HumanBodyBones.LeftFoot] = foot;
            tpose.rotation[(int)HumanBodyBones.RightUpperLeg] = rightLeg;
            tpose.rotation[(int)HumanBodyBones.RightLowerLeg] = rightLeg;
            tpose.rotation[(int)HumanBodyBones.RightFoot] = foot;
            tpose.forward = Vector3.forward;
            //missing stored
            //missing hipposition
            tpose.name = "SmartsuitTpose";
            return tpose;
        }
    }


}