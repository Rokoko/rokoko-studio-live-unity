using Rokoko.Core;
using Rokoko.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rokoko.Inputs
{
    public class Face : MonoBehaviour
    {
        [System.Serializable]
        public enum FaceMappingEnum
        {
            ARKit,
            Custom
        }

        private const int HEAD_TO_MATERIAL_INDEX = 3;

        [HideInInspector] public FaceMappingEnum blendshapeMapping;
        [HideInInspector] public BlendShapesMapping blendshapeCustomMap;

        [SerializeField] private SkinnedMeshRenderer meshRenderer = null;
        public bool debug = false;

        private Dictionary<string, int> blendshapeNamesToIndex = new Dictionary<string, int>();

        private void Awake()
        {
            if (meshRenderer == null)
            {
                Debug.LogError("Unassigned SkinnedMeshRenderer for face", this.transform);
                return;
            }

            for (int i = 0; i < meshRenderer.sharedMesh.blendShapeCount; i++)
            {
                blendshapeNamesToIndex.Add(meshRenderer.sharedMesh.GetBlendShapeName(i), i);
            }
        }

        public void UpdateFace(FaceFrame faceFrame)
        {
            if (meshRenderer == null) return;

            float[] blendshapeValues = faceFrame.GetValues();
            for (int i = 0; i < RokokoHelper.BlendshapesArray.Length; i++)
            {
                // Get blendshape name
                string blendShapeName;

                // Set default blendshape name
                if (blendshapeMapping == FaceMappingEnum.ARKit)
                {
                    blendShapeName = RokokoHelper.BlendshapesArray[i].ToString();
                }
                // Get custom blendshape name
                else
                {
                    blendShapeName = blendshapeCustomMap.blendshapeNames[RokokoHelper.BlendshapesArray[i]];
                }

                if (blendshapeNamesToIndex.ContainsKey(blendShapeName))
                    meshRenderer.SetBlendShapeWeight(blendshapeNamesToIndex[blendShapeName], blendshapeValues[i]);
                else
                {
                    if (debug)
                        Debug.LogWarning($"Couldn't find blendshape name:{blendShapeName} in Mesh blendshapes (count:{meshRenderer.sharedMesh.blendShapeCount})", this.transform);
                }
            }
        }

        public void SetColor(Color color)
        {
            if (meshRenderer == null) return;

            meshRenderer.materials[HEAD_TO_MATERIAL_INDEX].color = color;
        }
    }
}