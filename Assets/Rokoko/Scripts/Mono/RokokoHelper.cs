using Rokoko.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rokoko.Helper
{
    public static class RokokoHelper
    {
        public static Vector3 ToVector3(this Vector3Frame vec3)
        {
            return new Vector3(vec3.x, vec3.y, vec3.z);
        }

        public static Quaternion ToQuaternion(this Vector4Frame vec4)
        {
            return new Quaternion(vec4.x, vec4.y, vec4.z, vec4.w);
        }

        public static string ToLowerFirstChar(this string input)
        {
            string newString = input;
            if (!String.IsNullOrEmpty(newString) && Char.IsUpper(newString[0]))
                newString = Char.ToLower(newString[0]) + newString.Substring(1);
            return newString;
        }

        public static string ToUpperFirstChar(this string input)
        {
            string newString = input;
            if (!String.IsNullOrEmpty(newString) && Char.IsLower(newString[0]))
                newString = Char.ToUpper(newString[0]) + newString.Substring(1);
            return newString;
        }

        public static ActorJointFrame? GetBoneFrame(this BodyFrame frame, HumanBodyBones bone)
        {
            switch (bone)
            {
                case HumanBodyBones.Hips:
                    return frame.hip;
                case HumanBodyBones.LeftUpperLeg:
                    return frame.leftUpLeg;
                case HumanBodyBones.RightUpperLeg:
                    return frame.rightUpLeg;
                case HumanBodyBones.LeftLowerLeg:
                    return frame.leftLeg;
                case HumanBodyBones.RightLowerLeg:
                    return frame.rightLeg;
                case HumanBodyBones.LeftFoot:
                    return frame.leftFoot;
                case HumanBodyBones.RightFoot:
                    return frame.rightFoot;
                case HumanBodyBones.Spine:
                    return frame.spine;
                //case HumanBodyBones.Chest:
                //    return frame.chest;
                case HumanBodyBones.Neck:
                    return frame.neck;
                case HumanBodyBones.Head:
                    return frame.head;
                case HumanBodyBones.LeftShoulder:
                    return frame.leftShoulder;
                case HumanBodyBones.RightShoulder:
                    return frame.rightShoulder;
                case HumanBodyBones.LeftUpperArm:
                    return frame.leftUpperArm;
                case HumanBodyBones.RightUpperArm:
                    return frame.rightUpperArm;
                case HumanBodyBones.LeftLowerArm:
                    return frame.leftLowerArm;
                case HumanBodyBones.RightLowerArm:
                    return frame.rightLowerArm;
                case HumanBodyBones.LeftHand:
                    return frame.leftHand;
                case HumanBodyBones.RightHand:
                    return frame.rightHand;
                case HumanBodyBones.LeftToes:
                    return frame.leftToe;
                case HumanBodyBones.RightToes:
                    return frame.rightToe;
                //case HumanBodyBones.LeftEye:
                //    return frame.leftEye;
                //case HumanBodyBones.RightEye:
                //    return frame.rightEye;
                //case HumanBodyBones.Jaw:
                //    return frame.jaw;
                case HumanBodyBones.LeftThumbProximal:
                    return frame.leftThumbProximal;
                case HumanBodyBones.LeftThumbIntermediate:
                    return frame.leftThumbMedial;
                case HumanBodyBones.LeftThumbDistal:
                    return frame.leftThumbDistal;
                case HumanBodyBones.LeftIndexProximal:
                    return frame.leftIndexProximal;
                case HumanBodyBones.LeftIndexIntermediate:
                    return frame.leftIndexMedial;
                case HumanBodyBones.LeftIndexDistal:
                    return frame.leftIndexDistal;
                case HumanBodyBones.LeftMiddleProximal:
                    return frame.leftMiddleProximal;
                case HumanBodyBones.LeftMiddleIntermediate:
                    return frame.leftMiddleMedial;
                case HumanBodyBones.LeftMiddleDistal:
                    return frame.leftMiddleDistal;
                case HumanBodyBones.LeftRingProximal:
                    return frame.leftRingProximal;
                case HumanBodyBones.LeftRingIntermediate:
                    return frame.leftRingMedial;
                case HumanBodyBones.LeftRingDistal:
                    return frame.leftRingDistal;
                case HumanBodyBones.LeftLittleProximal:
                    return frame.leftLittleProximal;
                case HumanBodyBones.LeftLittleIntermediate:
                    return frame.leftLittleMedial;
                case HumanBodyBones.LeftLittleDistal:
                    return frame.leftLittleDistal;
                case HumanBodyBones.RightThumbProximal:
                    return frame.rightThumbProximal;
                case HumanBodyBones.RightThumbIntermediate:
                    return frame.rightThumbMedial;
                case HumanBodyBones.RightThumbDistal:
                    return frame.rightThumbDistal;
                case HumanBodyBones.RightIndexProximal:
                    return frame.rightIndexProximal;
                case HumanBodyBones.RightIndexIntermediate:
                    return frame.rightIndexMedial;
                case HumanBodyBones.RightIndexDistal:
                    return frame.rightIndexDistal;
                case HumanBodyBones.RightMiddleProximal:
                    return frame.rightMiddleProximal;
                case HumanBodyBones.RightMiddleIntermediate:
                    return frame.rightMiddleMedial;
                case HumanBodyBones.RightMiddleDistal:
                    return frame.rightMiddleDistal;
                case HumanBodyBones.RightRingProximal:
                    return frame.rightRingProximal;
                case HumanBodyBones.RightRingIntermediate:
                    return frame.rightRingMedial;
                case HumanBodyBones.RightRingDistal:
                    return frame.rightRingDistal;
                case HumanBodyBones.RightLittleProximal:
                    return frame.rightLittleProximal;
                case HumanBodyBones.RightLittleIntermediate:
                    return frame.rightLittleMedial;
                case HumanBodyBones.RightLittleDistal:
                    return frame.rightLittleDistal;
                case HumanBodyBones.UpperChest:
                    return frame.chest;
            }

            return null;
        }

        public static Color ToColor(this int[] color)
        {
            if (color == null || color.Length != 3)
                return Color.white;
            return new Color((float)color[0] / 255f, (float)color[1] / 255f, (float)color[2] / 255f);
        }

        public static void DestroyChildren(this Transform transform)
        {
            List<Transform> children = new List<Transform>();
            foreach (Transform child in transform)
                children.Add(child);

            foreach (Transform child in children)
                GameObject.Destroy(child.gameObject);
        }

        /// <summary>
        /// Check if actor name is present in live data
        /// </summary>
        public static bool HasProfile(this LiveFrame_v4 frame, string profileName)
        {
            for (int i = 0; i < frame.scene.actors.Length; i++)
            {
                if (frame.scene.actors[i].name == profileName)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check if character name is present in live data
        /// </summary>
        public static bool HasCharacter(this LiveFrame_v4 frame, string profileName)
        {
            for (int i = 0; i < frame.scene.characters.Length; i++)
            {
                if (frame.scene.characters[i].name == profileName)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check if prop name is present in live data
        /// </summary>
        public static bool HasProp(this LiveFrame_v4 frame, string propName)
        {
            for (int i = 0; i < frame.scene.props.Length; i++)
            {
                if (frame.scene.props[i].name == propName)
                    return true;
            }
            return false;
        }

        public static float[] GetValues(this FaceFrame faceFrame)
        {
            var values = new float[(int)BlendShapes.size];
            values[(int)BlendShapes.eyeBlinkLeft] = faceFrame.eyeBlinkLeft;
            values[(int)BlendShapes.eyeLookDownLeft] = faceFrame.eyeLookDownLeft;
            values[(int)BlendShapes.eyeLookInLeft] = faceFrame.eyeLookInLeft;
            values[(int)BlendShapes.eyeLookOutLeft] = faceFrame.eyeLookOutLeft;
            values[(int)BlendShapes.eyeLookUpLeft] = faceFrame.eyeLookUpLeft;
            values[(int)BlendShapes.eyeSquintLeft] = faceFrame.eyeSquintLeft;
            values[(int)BlendShapes.eyeWideLeft] = faceFrame.eyeWideLeft;
            values[(int)BlendShapes.eyeBlinkRight] = faceFrame.eyeBlinkRight;
            values[(int)BlendShapes.eyeLookDownRight] = faceFrame.eyeLookDownRight;
            values[(int)BlendShapes.eyeLookInRight] = faceFrame.eyeLookInRight;
            values[(int)BlendShapes.eyeLookOutRight] = faceFrame.eyeLookOutRight;
            values[(int)BlendShapes.eyeLookUpRight] = faceFrame.eyeLookUpRight;
            values[(int)BlendShapes.eyeSquintRight] = faceFrame.eyeSquintRight;
            values[(int)BlendShapes.eyeWideRight] = faceFrame.eyeWideRight;
            values[(int)BlendShapes.jawForward] = faceFrame.jawForward;
            values[(int)BlendShapes.jawLeft] = faceFrame.jawLeft;
            values[(int)BlendShapes.jawRight] = faceFrame.jawRight;
            values[(int)BlendShapes.jawOpen] = faceFrame.jawOpen;
            values[(int)BlendShapes.mouthClose] = faceFrame.mouthClose;
            values[(int)BlendShapes.mouthFunnel] = faceFrame.mouthFunnel;
            values[(int)BlendShapes.mouthPucker] = faceFrame.mouthPucker;
            values[(int)BlendShapes.mouthLeft] = faceFrame.mouthLeft;
            values[(int)BlendShapes.mouthRight] = faceFrame.mouthRight;
            values[(int)BlendShapes.mouthSmileLeft] = faceFrame.mouthSmileLeft;
            values[(int)BlendShapes.mouthSmileRight] = faceFrame.mouthSmileRight;
            values[(int)BlendShapes.mouthFrownLeft] = faceFrame.mouthFrownLeft;
            values[(int)BlendShapes.mouthFrownRight] = faceFrame.mouthFrownRight;
            values[(int)BlendShapes.mouthDimpleLeft] = faceFrame.mouthDimpleLeft;
            values[(int)BlendShapes.mouthDimpleRight] = faceFrame.mouthDimpleRight;
            values[(int)BlendShapes.mouthStretchLeft] = faceFrame.mouthStretchLeft;
            values[(int)BlendShapes.mouthStretchRight] = faceFrame.mouthStretchRight;
            values[(int)BlendShapes.mouthRollLower] = faceFrame.mouthRollLower;
            values[(int)BlendShapes.mouthRollUpper] = faceFrame.mouthRollUpper;
            values[(int)BlendShapes.mouthShrugLower] = faceFrame.mouthShrugLower;
            values[(int)BlendShapes.mouthShrugUpper] = faceFrame.mouthShrugUpper;
            values[(int)BlendShapes.mouthPressLeft] = faceFrame.mouthPressLeft;
            values[(int)BlendShapes.mouthPressRight] = faceFrame.mouthPressRight;
            values[(int)BlendShapes.mouthLowerDownLeft] = faceFrame.mouthLowerDownLeft;
            values[(int)BlendShapes.mouthLowerDownRight] = faceFrame.mouthLowerDownRight;
            values[(int)BlendShapes.mouthUpperUpLeft] = faceFrame.mouthUpperUpLeft;
            values[(int)BlendShapes.mouthUpperUpRight] = faceFrame.mouthUpperUpRight;
            values[(int)BlendShapes.browDownLeft] = faceFrame.browDownLeft;
            values[(int)BlendShapes.browDownRight] = faceFrame.browDownRight;
            values[(int)BlendShapes.browInnerUp] = faceFrame.browInnerUp;
            values[(int)BlendShapes.browOuterUpLeft] = faceFrame.browOuterUpLeft;
            values[(int)BlendShapes.browOuterUpRight] = faceFrame.browOuterUpRight;
            values[(int)BlendShapes.cheekPuff] = faceFrame.cheekPuff;
            values[(int)BlendShapes.cheekSquintLeft] = faceFrame.cheekSquintLeft;
            values[(int)BlendShapes.cheekSquintRight] = faceFrame.cheekSquintRight;
            values[(int)BlendShapes.noseSneerLeft] = faceFrame.noseSneerLeft;
            values[(int)BlendShapes.noseSneerRight] = faceFrame.noseSneerRight;
            values[(int)BlendShapes.tongueOut] = faceFrame.tongueOut;

            return values;
        }

        public static Dictionary<string, int> GetAllBlendshapes(this Mesh mesh)
        {
            Dictionary<string, int> blendshapeNamesToIndex = new Dictionary<string, int>();
            for (int i = 0; i < mesh.blendShapeCount; i++)
            {
                blendshapeNamesToIndex.Add(mesh.GetBlendShapeName(i).ToLower(), i);
            }
            return blendshapeNamesToIndex;
        }

        /// <summary>
        /// Get all missing blendshapes comparing to ARKit 52 blendshapes
        /// </summary>
        public static List<string> GetAllMissingBlendshapes(this Mesh mesh)
        {
            List<string> missingBlendshapes = new List<string>();
            List<string> blendshapeNames = new List<string>(mesh.GetAllBlendshapes().Keys);
            for (int i = 0; i < BlendshapesArray.Length; i++)
            {
                string arkitName = BlendshapesArray[i].ToString();
                if (!blendshapeNames.Contains(arkitName.ToLower()))
                    missingBlendshapes.Add(arkitName);
            }

            return missingBlendshapes;
        }

        private static BlendShapes[] _BlendshapesArray = null;
        public static BlendShapes[] BlendshapesArray
        {
            get
            {
                if (_BlendshapesArray == null)
                {
                    _BlendshapesArray = new BlendShapes[(int)BlendShapes.size];
                    for (int i = 0; i < _BlendshapesArray.Length; i++)
                    {
                        _BlendshapesArray[i] = (BlendShapes)i;
                    }
                }

                return _BlendshapesArray;
            }
        }

        private static HumanBodyBones[] _HumanBodyBonesArray = null;

        public static HumanBodyBones[] HumanBodyBonesArray
        {
            get
            {
                if (_HumanBodyBonesArray == null)
                {
                    _HumanBodyBonesArray = new HumanBodyBones[(int)HumanBodyBones.LastBone];
                    for (int i = 0; i < _HumanBodyBonesArray.Length; i++)
                        _HumanBodyBonesArray[i] = (HumanBodyBones)i;
                }
                return _HumanBodyBonesArray;
            }
        }

        public static void Destroy(GameObject gameObject)
        {
            if (Application.isPlaying)
                GameObject.Destroy(gameObject);
            else
                GameObject.DestroyImmediate(gameObject);

#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
#endif
        }
    }
}