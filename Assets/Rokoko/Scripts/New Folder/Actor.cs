using Rokoko.Serializers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rokoko
{
    [RequireComponent(typeof(Animator))]
    public class Actor : MonoBehaviour
    {
        private const int HEAD_TO_MATERIAL_INDEX = 5;
        private const int JOINT_TO_MATERIAL_INDEX = 1;
        public string actorName { get; private set; }

        [SerializeField] private Renderer meshRenderer = null;
        [SerializeField] private Face face = null;
        [SerializeField] private Material bodyMaterial = null;
        [SerializeField] private Material invisibleMaterial = null;

        private Dictionary<HumanBodyBones, Transform> humanBones = new Dictionary<HumanBodyBones, Transform>();
        private Animator animator;
        private Material[] meshMaterials;


        private void Awake()
        {
            animator = this.GetComponent<Animator>();
            InitializeBodyBones();
            InitializeMaterials();
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

        private void InitializeMaterials()
        {
            // Clone the material, so not to affect other objects
            bodyMaterial = Material.Instantiate(bodyMaterial);
            meshMaterials = new Material[meshRenderer.materials.Length];
            for (int i = 0; i < meshMaterials.Length; i++)
            {
                // Keep joint material as source
                if (i == JOINT_TO_MATERIAL_INDEX)
                    meshMaterials[i] = meshRenderer.materials[i];
                else
                    meshMaterials[i] = bodyMaterial;
            }
            meshRenderer.materials = meshMaterials;
        }

        public void UpdateActor(ActorFrame actorFrame)
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

            // Update material color and visibility
            UpdateMaterialColors(actorFrame);
        }

        private void UpdateMaterialColors(ActorFrame actorFrame)
        {
            bodyMaterial.color = actorFrame.color.ToColor();
            meshMaterials[HEAD_TO_MATERIAL_INDEX] = (actorFrame.meta.hasFace) ? invisibleMaterial : bodyMaterial;
            meshRenderer.materials = meshMaterials;

            face.SetColor(actorFrame.color.ToColor());
        }

        private void UpdateSkeleton(BodyFrame bodyFrame)
        {
            foreach (HumanBodyBones bone in System.Enum.GetValues(typeof(HumanBodyBones)))
            {
                if (bone == HumanBodyBones.LastBone) break;
                ActorJointFrame? boneFrame = bodyFrame.GetBoneFrame(bone);
                if (boneFrame != null)
                    UpdateBone(bone, boneFrame.Value);
            }
        }

        private void UpdateBone(HumanBodyBones bone, ActorJointFrame jointFrame)
        {
            humanBones[bone].position = jointFrame.position.ToVector3();
            humanBones[bone].rotation = jointFrame.rotation.ToQuaternion();
        }
    }
}