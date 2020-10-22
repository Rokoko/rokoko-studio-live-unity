using Rokoko.Serializers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rokoko
{
    public class ActorNewton : Actor
    {
        private const int HEAD_TO_MATERIAL_INDEX = 5;
        private const int JOINT_TO_MATERIAL_INDEX = 1;

        [SerializeField] private Material bodyMaterial = null;
        [SerializeField] private Material invisibleMaterial = null;

        #region Initialize

        protected override void Awake()
        {
            base.Awake();
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

        #endregion

        #region Public Methods

        public override void UpdateActor(ActorFrame actorFrame)
        {
            base.UpdateActor(actorFrame);

            // Update material color and visibility
            UpdateMaterialColors(actorFrame);
        }

        #endregion

        #region Internal Logic

        private void UpdateMaterialColors(ActorFrame actorFrame)
        {
            bodyMaterial.color = actorFrame.color.ToColor();
            meshMaterials[HEAD_TO_MATERIAL_INDEX] = (actorFrame.meta.hasFace) ? invisibleMaterial : bodyMaterial;
            meshRenderer.materials = meshMaterials;

            face.SetColor(actorFrame.color.ToColor());
        }

        #endregion

    }
}