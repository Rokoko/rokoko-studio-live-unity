using Rokoko.Core;
using Rokoko.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rokoko.Inputs
{
    public class ActorNewton : Actor
    {
        private const int HEAD_TO_MATERIAL_INDEX = 5;
        private const int JOINT_TO_MATERIAL_INDEX = 1;

        [Header("Newton materials")]
        [SerializeField] protected Renderer meshRenderer = null;
        [SerializeField] private Material bodyMaterial = null;
        [SerializeField] private Material faceInvisibleMaterial = null;
        public bool autoHideFaceWhenInactive = false;

        protected Material[] meshMaterials;

        #region Initialize

        protected override void Awake()
        {
            base.Awake();
            InitializeMaterials();
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

        public override void CreateIdle(string actorName)
        {
            base.CreateIdle(actorName);

            if (autoHideFaceWhenInactive)
                face?.gameObject.SetActive(false);
        }

        public override void UpdateActor(ActorFrame actorFrame)
        {
            base.UpdateActor(actorFrame);

            bool updateBody = actorFrame.meta.hasBody || actorFrame.meta.hasGloves;

            // Enable/Disable body renderer
            meshRenderer.enabled = updateBody;

            // Update material color and visibility
            UpdateMaterialColors(actorFrame);

            // Enable/Disable face renderer
            if (autoHideFaceWhenInactive)
                face?.gameObject.SetActive(actorFrame.meta.hasFace);
        }

        #endregion

        #region Internal Logic

        private void UpdateMaterialColors(ActorFrame actorFrame)
        {
            bodyMaterial.color = actorFrame.color.ToColor();
            meshMaterials[HEAD_TO_MATERIAL_INDEX] = (actorFrame.meta.hasFace) ? faceInvisibleMaterial : bodyMaterial;
            meshRenderer.materials = meshMaterials;

            face?.SetColor(actorFrame.color.ToColor());
        }

        #endregion

    }
}