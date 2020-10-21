using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rokoko.VirtualProduction
{
    public class Face : MonoBehaviour
    {
        public string faceId = "";

        [SerializeField] private SkinnedMeshRenderer _faceSkin;
        [SerializeField] private bool logWarnings;
        [Space(10)]
        [SerializeField] private FaceBlendshapeMap _blendshapeMapping;
        
        private List<string> blendshapeNames;
        private void Reset()
        {
            if (!_faceSkin) _faceSkin = GetComponent<SkinnedMeshRenderer>();
            var blendshapes = GetBlendShapeNames();
            foreach (var blendshape in blendshapes)
            {
                if (blendshape.Contains("eyeBlinkLeft"))
                {
                    _blendshapeMapping.eyeBlinkLeft = blendshape;
                } else if (blendshape.Contains("eyeLookDownLeft")) {
                    _blendshapeMapping.eyeLookDownLeft = blendshape;
                } else if (blendshape.Contains("eyeLookInLeft")) {
                    _blendshapeMapping.eyeLookInLeft = blendshape;
                } else if (blendshape.Contains("eyeLookOutLeft")) {
                    _blendshapeMapping.eyeLookOutLeft = blendshape;
                } else if (blendshape.Contains("eyeLookUpLeft")) {
                    _blendshapeMapping.eyeLookUpLeft = blendshape;
                } else if (blendshape.Contains("eyeSquintLeft")) {
                    _blendshapeMapping.eyeSquintLeft = blendshape;
                } else if (blendshape.Contains("eyeWideLeft")) {
                    _blendshapeMapping.eyeWideLeft = blendshape;
                } else if (blendshape.Contains("eyeBlinkRight")) {
                    _blendshapeMapping.eyeBlinkRight = blendshape;
                } else if (blendshape.Contains("eyeLookDownRight")) {
                    _blendshapeMapping.eyeLookDownRight = blendshape;
                } else if (blendshape.Contains("eyeLookInRight")) {
                    _blendshapeMapping.eyeLookInRight = blendshape;
                } else if (blendshape.Contains("eyeLookOutRight")) {
                    _blendshapeMapping.eyeLookOutRight = blendshape;
                } else if (blendshape.Contains("eyeLookUpRight")) {
                    _blendshapeMapping.eyeLookUpRight = blendshape;
                } else if (blendshape.Contains("eyeSquintRight")) {
                    _blendshapeMapping.eyeSquintRight = blendshape;
                } else if (blendshape.Contains("eyeWideRight")) {
                    _blendshapeMapping.eyeWideRight = blendshape;
                } else if (blendshape.Contains("jawForward")) {
                    _blendshapeMapping.jawForward = blendshape;
                } else if (blendshape.Contains("jawLeft")) {
                    _blendshapeMapping.jawLeft = blendshape;
                } else if (blendshape.Contains("jawRight")) {
                    _blendshapeMapping.jawRight = blendshape;
                } else if (blendshape.Contains("jawOpen")) {
                    _blendshapeMapping.jawOpen = blendshape;
                } else if (blendshape.Contains("mouthClose")) {
                    _blendshapeMapping.mouthClose = blendshape;
                } else if (blendshape.Contains("mouthFunnel")) {
                    _blendshapeMapping.mouthFunnel = blendshape;
                } else if (blendshape.Contains("mouthPucker")) {
                    _blendshapeMapping.mouthPucker = blendshape;
                } else if (blendshape.Contains("mouthLeft")) {
                    _blendshapeMapping.mouthLeft = blendshape;
                } else if (blendshape.Contains("mouthRight")) {
                    _blendshapeMapping.mouthRight = blendshape;
                } else if (blendshape.Contains("mouthSmileLeft")) {
                    _blendshapeMapping.mouthSmileLeft = blendshape;
                } else if (blendshape.Contains("mouthSmileRight")) {
                    _blendshapeMapping.mouthSmileRight = blendshape;
                } else if (blendshape.Contains("mouthFrownLeft")) {
                    _blendshapeMapping.mouthFrownLeft = blendshape;
                } else if (blendshape.Contains("mouthFrownRight")) {
                    _blendshapeMapping.mouthFrownRight = blendshape;
                } else if (blendshape.Contains("mouthDimpleLeft")) {
                    _blendshapeMapping.mouthDimpleLeft = blendshape;
                } else if (blendshape.Contains("mouthDimpleRight")) {
                    _blendshapeMapping.mouthDimpleRight = blendshape;
                } else if (blendshape.Contains("mouthStretchLeft")) {
                    _blendshapeMapping.mouthStretchLeft = blendshape;
                } else if (blendshape.Contains("mouthStretchRight")) {
                    _blendshapeMapping.mouthStretchRight = blendshape;
                } else if (blendshape.Contains("mouthRollLower")) {
                    _blendshapeMapping.mouthRollLower = blendshape;
                } else if (blendshape.Contains("mouthRollUpper")) {
                    _blendshapeMapping.mouthRollUpper = blendshape;
                } else if (blendshape.Contains("mouthShrugLower")) {
                    _blendshapeMapping.mouthShrugLower = blendshape;
                } else if (blendshape.Contains("mouthShrugUpper")) {
                    _blendshapeMapping.mouthShrugUpper = blendshape;
                } else if (blendshape.Contains("mouthPressLeft")) {
                    _blendshapeMapping.mouthPressLeft = blendshape;
                } else if (blendshape.Contains("mouthPressRight")) {
                    _blendshapeMapping.mouthPressRight = blendshape;
                } else if (blendshape.Contains("mouthLowerDownLeft")) {
                    _blendshapeMapping.mouthLowerDownLeft = blendshape;
                } else if (blendshape.Contains("mouthLowerDownRight")) {
                    _blendshapeMapping.mouthLowerDownRight = blendshape;
                } else if (blendshape.Contains("mouthUpperUpLeft")) {
                    _blendshapeMapping.mouthUpperUpLeft = blendshape;
                } else if (blendshape.Contains("mouthUpperUpRight")) {
                    _blendshapeMapping.mouthUpperUpRight = blendshape;
                } else if (blendshape.Contains("browDownLeft")) {
                    _blendshapeMapping.browDownLeft = blendshape;
                } else if (blendshape.Contains("browDownRight")) {
                    _blendshapeMapping.browDownRight = blendshape;
                } else if (blendshape.Contains("browInnerUp")) {
                    _blendshapeMapping.browInnerUp = blendshape;
                } else if (blendshape.Contains("browOuterUpLeft")) {
                    _blendshapeMapping.browOuterUpLeft = blendshape;
                } else if (blendshape.Contains("browOuterUpRight")) {
                    _blendshapeMapping.browOuterUpRight = blendshape;
                } else if (blendshape.Contains("cheekPuff")) {
                    _blendshapeMapping.cheekPuff = blendshape;
                } else if (blendshape.Contains("cheekSquintLeft")) {
                    _blendshapeMapping.cheekSquintLeft = blendshape;
                } else if (blendshape.Contains("cheekSquintRight")) {
                    _blendshapeMapping.cheekSquintRight = blendshape;
                } else if (blendshape.Contains("noseSneerLeft")) {
                    _blendshapeMapping.noseSneerLeft = blendshape;
                } else if (blendshape.Contains("noseSneerRight")) {
                    _blendshapeMapping.noseSneerRight = blendshape;
                } else if (blendshape.Contains("tongueOut")) {
                    _blendshapeMapping.tongueOut = blendshape;
                }
            }
        }

        private void Start()
        {
            blendshapeNames = new List<string>();
            blendshapeNames.AddRange(GetBlendShapeNames());
        }

        private void Update()
        {
            foreach (var face in VirtualProductionReceiver.Instance.VirtualProductionData.faces)
            {
                if (face.faceId == faceId)
                {
                    ApplyFace(face);
                    break;
                }
            }
        }

        private void ApplyFace(FaceData face)
        {
            ApplyBlendShape(_blendshapeMapping.eyeBlinkLeft, blendshapeNames.IndexOf(_blendshapeMapping.eyeBlinkLeft), face.eyeBlinkLeft);
            ApplyBlendShape(_blendshapeMapping.eyeLookDownLeft, blendshapeNames.IndexOf(_blendshapeMapping.eyeLookDownLeft), face.eyeLookDownLeft);    
            ApplyBlendShape(_blendshapeMapping.eyeLookInLeft, blendshapeNames.IndexOf(_blendshapeMapping.eyeLookInLeft), face.eyeLookInLeft);    
            ApplyBlendShape(_blendshapeMapping.eyeLookOutLeft, blendshapeNames.IndexOf(_blendshapeMapping.eyeLookOutLeft), face.eyeLookOutLeft);    
            ApplyBlendShape(_blendshapeMapping.eyeLookUpLeft, blendshapeNames.IndexOf(_blendshapeMapping.eyeLookUpLeft), face.eyeLookUpLeft);    
            ApplyBlendShape(_blendshapeMapping.eyeSquintLeft, blendshapeNames.IndexOf(_blendshapeMapping.eyeSquintLeft), face.eyeSquintLeft);    
            ApplyBlendShape(_blendshapeMapping.eyeWideLeft, blendshapeNames.IndexOf(_blendshapeMapping.eyeWideLeft), face.eyeWideLeft);    
            ApplyBlendShape(_blendshapeMapping.eyeBlinkRight, blendshapeNames.IndexOf(_blendshapeMapping.eyeBlinkRight), face.eyeBlinkRight);    
            ApplyBlendShape(_blendshapeMapping.eyeLookDownRight, blendshapeNames.IndexOf(_blendshapeMapping.eyeLookDownRight), face.eyeLookDownRight);    
            ApplyBlendShape(_blendshapeMapping.eyeLookInRight, blendshapeNames.IndexOf(_blendshapeMapping.eyeLookInRight), face.eyeLookInRight);    
            ApplyBlendShape(_blendshapeMapping.eyeLookOutRight, blendshapeNames.IndexOf(_blendshapeMapping.eyeLookOutRight), face.eyeLookOutRight);    
            ApplyBlendShape(_blendshapeMapping.eyeLookUpRight, blendshapeNames.IndexOf(_blendshapeMapping.eyeLookUpRight), face.eyeLookUpRight);    
            ApplyBlendShape(_blendshapeMapping.eyeSquintRight, blendshapeNames.IndexOf(_blendshapeMapping.eyeSquintRight), face.eyeSquintRight);    
            ApplyBlendShape(_blendshapeMapping.eyeWideRight, blendshapeNames.IndexOf(_blendshapeMapping.eyeWideRight), face.eyeWideRight);    
            ApplyBlendShape(_blendshapeMapping.jawForward, blendshapeNames.IndexOf(_blendshapeMapping.jawForward), face.jawForward);    
            ApplyBlendShape(_blendshapeMapping.jawLeft, blendshapeNames.IndexOf(_blendshapeMapping.jawLeft), face.jawLeft);    
            ApplyBlendShape(_blendshapeMapping.jawRight, blendshapeNames.IndexOf(_blendshapeMapping.jawRight), face.jawRight);    
            ApplyBlendShape(_blendshapeMapping.jawOpen, blendshapeNames.IndexOf(_blendshapeMapping.jawOpen), face.jawOpen);    
            ApplyBlendShape(_blendshapeMapping.mouthClose, blendshapeNames.IndexOf(_blendshapeMapping.mouthClose), face.mouthClose);    
            ApplyBlendShape(_blendshapeMapping.mouthFunnel, blendshapeNames.IndexOf(_blendshapeMapping.mouthFunnel), face.mouthFunnel);    
            ApplyBlendShape(_blendshapeMapping.mouthPucker, blendshapeNames.IndexOf(_blendshapeMapping.mouthPucker), face.mouthPucker);    
            ApplyBlendShape(_blendshapeMapping.mouthLeft, blendshapeNames.IndexOf(_blendshapeMapping.mouthLeft), face.mouthLeft);    
            ApplyBlendShape(_blendshapeMapping.mouthRight, blendshapeNames.IndexOf(_blendshapeMapping.mouthRight), face.mouthRight);    
            ApplyBlendShape(_blendshapeMapping.mouthSmileLeft, blendshapeNames.IndexOf(_blendshapeMapping.mouthSmileLeft), face.mouthSmileLeft);    
            ApplyBlendShape(_blendshapeMapping.mouthSmileRight, blendshapeNames.IndexOf(_blendshapeMapping.mouthSmileRight), face.mouthSmileRight);    
            ApplyBlendShape(_blendshapeMapping.mouthFrownLeft, blendshapeNames.IndexOf(_blendshapeMapping.mouthFrownLeft), face.mouthFrownLeft);    
            ApplyBlendShape(_blendshapeMapping.mouthFrownRight, blendshapeNames.IndexOf(_blendshapeMapping.mouthFrownRight), face.mouthFrownRight);    
            ApplyBlendShape(_blendshapeMapping.mouthDimpleLeft, blendshapeNames.IndexOf(_blendshapeMapping.mouthDimpleLeft), face.mouthDimpleLeft);    
            ApplyBlendShape(_blendshapeMapping.mouthDimpleRight, blendshapeNames.IndexOf(_blendshapeMapping.mouthDimpleRight), face.mouthDimpleRight);    
            ApplyBlendShape(_blendshapeMapping.mouthStretchLeft, blendshapeNames.IndexOf(_blendshapeMapping.mouthStretchLeft), face.mouthStretchLeft);    
            ApplyBlendShape(_blendshapeMapping.mouthStretchRight, blendshapeNames.IndexOf(_blendshapeMapping.mouthStretchRight), face.mouthStretchRight);    
            ApplyBlendShape(_blendshapeMapping.mouthRollLower, blendshapeNames.IndexOf(_blendshapeMapping.mouthRollLower), face.mouthRollLower);    
            ApplyBlendShape(_blendshapeMapping.mouthRollUpper, blendshapeNames.IndexOf(_blendshapeMapping.mouthRollUpper), face.mouthRollUpper);    
            ApplyBlendShape(_blendshapeMapping.mouthShrugLower, blendshapeNames.IndexOf(_blendshapeMapping.mouthShrugLower), face.mouthShrugLower);    
            ApplyBlendShape(_blendshapeMapping.mouthShrugUpper, blendshapeNames.IndexOf(_blendshapeMapping.mouthShrugUpper), face.mouthShrugUpper);    
            ApplyBlendShape(_blendshapeMapping.mouthPressLeft, blendshapeNames.IndexOf(_blendshapeMapping.mouthPressLeft), face.mouthPressLeft);    
            ApplyBlendShape(_blendshapeMapping.mouthPressRight, blendshapeNames.IndexOf(_blendshapeMapping.mouthPressRight), face.mouthPressRight);    
            ApplyBlendShape(_blendshapeMapping.mouthLowerDownLeft, blendshapeNames.IndexOf(_blendshapeMapping.mouthLowerDownLeft), face.mouthLowerDownLeft);    
            ApplyBlendShape(_blendshapeMapping.mouthLowerDownRight, blendshapeNames.IndexOf(_blendshapeMapping.mouthLowerDownRight), face.mouthLowerDownRight);    
            ApplyBlendShape(_blendshapeMapping.mouthUpperUpLeft, blendshapeNames.IndexOf(_blendshapeMapping.mouthUpperUpLeft), face.mouthUpperUpLeft);    
            ApplyBlendShape(_blendshapeMapping.mouthUpperUpRight, blendshapeNames.IndexOf(_blendshapeMapping.mouthUpperUpRight), face.mouthUpperUpRight);    
            ApplyBlendShape(_blendshapeMapping.browDownLeft, blendshapeNames.IndexOf(_blendshapeMapping.browDownLeft), face.browDownLeft);    
            ApplyBlendShape(_blendshapeMapping.browDownRight, blendshapeNames.IndexOf(_blendshapeMapping.browDownRight), face.browDownRight);    
            ApplyBlendShape(_blendshapeMapping.browInnerUp, blendshapeNames.IndexOf(_blendshapeMapping.browInnerUp), face.browInnerUp);    
            ApplyBlendShape(_blendshapeMapping.browOuterUpLeft, blendshapeNames.IndexOf(_blendshapeMapping.browOuterUpLeft), face.browOuterUpLeft);    
            ApplyBlendShape(_blendshapeMapping.browOuterUpRight, blendshapeNames.IndexOf(_blendshapeMapping.browOuterUpRight), face.browOuterUpRight);    
            ApplyBlendShape(_blendshapeMapping.cheekPuff, blendshapeNames.IndexOf(_blendshapeMapping.cheekPuff), face.cheekPuff);    
            ApplyBlendShape(_blendshapeMapping.cheekSquintLeft, blendshapeNames.IndexOf(_blendshapeMapping.cheekSquintLeft), face.cheekSquintLeft);    
            ApplyBlendShape(_blendshapeMapping.cheekSquintRight, blendshapeNames.IndexOf(_blendshapeMapping.cheekSquintRight), face.cheekSquintRight);    
            ApplyBlendShape(_blendshapeMapping.noseSneerLeft, blendshapeNames.IndexOf(_blendshapeMapping.noseSneerLeft), face.noseSneerLeft);    
            ApplyBlendShape(_blendshapeMapping.noseSneerRight, blendshapeNames.IndexOf(_blendshapeMapping.noseSneerRight), face.noseSneerRight);    
            ApplyBlendShape(_blendshapeMapping.tongueOut, blendshapeNames.IndexOf(_blendshapeMapping.tongueOut), face.tongueOut);    
        }

        private void ApplyBlendShape(string blendshapeName, int blendshapeIndex, float value)
        {
            if (blendshapeIndex >= 0)
            {
                _faceSkin.SetBlendShapeWeight(blendshapeIndex, value);
            }
            else if (logWarnings)
            {
                Debug.LogWarning($"{blendshapeName} not found");
            }
        }

        public string[] GetBlendShapeNames()
        {
            Mesh m = _faceSkin.sharedMesh;
            string[] arr;
            arr = new string [m.blendShapeCount];
            for (int i = 0; i < m.blendShapeCount; i++)
            {
                string s = m.GetBlendShapeName(i);
                arr[i] = s;
            }

            return arr;
        }

        [System.Serializable]
        public struct FaceBlendshapeMap
        {
            public string eyeBlinkLeft; // 4
            public string eyeLookDownLeft; // 4
            public string eyeLookInLeft; // 4
            public string eyeLookOutLeft; // 4
            public string eyeLookUpLeft;
            public string eyeSquintLeft;
            public string eyeWideLeft;
            public string eyeBlinkRight;
            public string eyeLookDownRight;
            public string eyeLookInRight;
            public string eyeLookOutRight;
            public string eyeLookUpRight;
            public string eyeSquintRight;
            public string eyeWideRight;
            public string jawForward;
            public string jawLeft;
            public string jawRight;
            public string jawOpen;
            public string mouthClose;
            public string mouthFunnel;
            public string mouthPucker;
            public string mouthLeft;
            public string mouthRight;
            public string mouthSmileLeft;
            public string mouthSmileRight;
            public string mouthFrownLeft;
            public string mouthFrownRight;
            public string mouthDimpleLeft;
            public string mouthDimpleRight;
            public string mouthStretchLeft;
            public string mouthStretchRight;
            public string mouthRollLower;
            public string mouthRollUpper;
            public string mouthShrugLower;
            public string mouthShrugUpper;
            public string mouthPressLeft;
            public string mouthPressRight;
            public string mouthLowerDownLeft;
            public string mouthLowerDownRight;
            public string mouthUpperUpLeft;
            public string mouthUpperUpRight;
            public string browDownLeft;
            public string browDownRight;
            public string browInnerUp;
            public string browOuterUpLeft;
            public string browOuterUpRight;
            public string cheekPuff;
            public string cheekSquintLeft;
            public string cheekSquintRight;
            public string noseSneerLeft;
            public string noseSneerRight;
            public string tongueOut;
        }
    }
}