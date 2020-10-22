using Rokoko.Serializers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rokoko
{
    public class Face : MonoBehaviour
    {
        private const int HEAD_TO_MATERIAL_INDEX = 3;

        [SerializeField] private SkinnedMeshRenderer meshRenderer = null;

        private void Awake()
        {
            if(meshRenderer == null)
            {
                Debug.LogError("Unassigned SkinnedMeshRenderer for face", this.transform);
            }
        }

        public void UpdateFace(FaceFrame faceFrame)
        {
            IReadOnlyList<string> blendshapeNames = faceFrame.GetBlendShapes();
            float[] blendshapeValues = faceFrame.GetValues();
            for (int i = 0; i < blendshapeNames.Count; i++)
            {
                meshRenderer.SetBlendShapeWeight(i, blendshapeValues[i]);
            }
        }

        public void SetColor(Color color)
        {
            meshRenderer.materials[HEAD_TO_MATERIAL_INDEX].color = color;
        }
    }
}