using System.Collections.Generic;
using UnityEngine;

namespace Rokoko.Core
{
    #region Data Structs

    [System.Serializable]
    public class LiveFrame_v4
    {
        // Header
        public float version;
        // Target fps
        public float fps;
        public SceneFrame scene;
    }

    [System.Serializable]
    public struct SceneFrame
    {
        public float timestamp;
        public ActorFrame[] actors;
        public CharacterFrame[] characters;
        public PropFrame[] props;
    }

    [System.Serializable]
    public struct ActorFrame
    {
        public string name;
        public int[] color;
        public Meta meta;
        public Dimensions dimensions;

        public BodyFrame body;
        public FaceFrame face;

        [System.Serializable]
        public struct Meta
        {
            public bool hasGloves;
            public bool hasLeftGlove;
            public bool hasRightGlove;
            public bool hasBody;
            public bool hasFace;
        }

        [System.Serializable]
        public struct Dimensions
        {
            public float totalHeight;
            public float hipHeight;
        }
    }

    [System.Serializable]
    public class CharacterFrame
    {
        public string name;
        public CharacterJointFrame[] joints;
        public BlendshapesFrame blendshapes;
    }

    [System.Serializable]
    public class BlendshapesFrame
    {
        public string[] names;
        public float[] values;
    }

    [System.Serializable]
    public class PropFrame
    {
        public string name;
        public int[] color;
        public int type;
        public Vector3Frame position;
        public Vector4Frame rotation;
    }


    [System.Serializable]
    public class BodyFrame
    {
        public ActorJointFrame hip;
        public ActorJointFrame spine;
        public ActorJointFrame chest;
        public ActorJointFrame neck;
        public ActorJointFrame head;
        public ActorJointFrame leftShoulder;
        public ActorJointFrame leftUpperArm;
        public ActorJointFrame leftLowerArm;
        public ActorJointFrame leftHand;
        public ActorJointFrame rightShoulder;
        public ActorJointFrame rightUpperArm;
        public ActorJointFrame rightLowerArm;
        public ActorJointFrame rightHand;

        public ActorJointFrame leftUpLeg;
        public ActorJointFrame leftLeg;
        public ActorJointFrame leftFoot;
        public ActorJointFrame leftToe;
        public ActorJointFrame leftToeEnd;

        public ActorJointFrame rightUpLeg;
        public ActorJointFrame rightLeg;
        public ActorJointFrame rightFoot;
        public ActorJointFrame rightToe;
        public ActorJointFrame rightToeEnd;


        public ActorJointFrame leftThumbProximal;
        public ActorJointFrame leftThumbMedial;
        public ActorJointFrame leftThumbDistal;
        public ActorJointFrame leftThumbTip;

        public ActorJointFrame leftIndexProximal;
        public ActorJointFrame leftIndexMedial;
        public ActorJointFrame leftIndexDistal;
        public ActorJointFrame leftIndexTip;

        public ActorJointFrame leftMiddleProximal;
        public ActorJointFrame leftMiddleMedial;
        public ActorJointFrame leftMiddleDistal;
        public ActorJointFrame leftMiddleTip;

        public ActorJointFrame leftRingProximal;
        public ActorJointFrame leftRingMedial;
        public ActorJointFrame leftRingDistal;
        public ActorJointFrame leftRingTip;

        public ActorJointFrame leftLittleProximal;
        public ActorJointFrame leftLittleMedial;
        public ActorJointFrame leftLittleDistal;
        public ActorJointFrame leftLittleTip;


        public ActorJointFrame rightThumbProximal;
        public ActorJointFrame rightThumbMedial;
        public ActorJointFrame rightThumbDistal;
        public ActorJointFrame rightThumbTip;

        public ActorJointFrame rightIndexProximal;
        public ActorJointFrame rightIndexMedial;
        public ActorJointFrame rightIndexDistal;
        public ActorJointFrame rightIndexTip;

        public ActorJointFrame rightMiddleProximal;
        public ActorJointFrame rightMiddleMedial;
        public ActorJointFrame rightMiddleDistal;
        public ActorJointFrame rightMiddleTip;
        public ActorJointFrame rightRingProximal;
        public ActorJointFrame rightRingMedial;
        public ActorJointFrame rightRingDistal;
        public ActorJointFrame rightRingTip;

        public ActorJointFrame rightLittleProximal;
        public ActorJointFrame rightLittleMedial;
        public ActorJointFrame rightLittleDistal;
        public ActorJointFrame rightLittleTip;
    }

    [System.Serializable]
    public class FaceFrame
    {
        public string faceId;
        public float eyeBlinkLeft;
        public float eyeLookDownLeft;
        public float eyeLookInLeft;
        public float eyeLookOutLeft;
        public float eyeLookUpLeft;
        public float eyeSquintLeft;
        public float eyeWideLeft;
        public float eyeBlinkRight;
        public float eyeLookDownRight;
        public float eyeLookInRight;
        public float eyeLookOutRight;
        public float eyeLookUpRight;
        public float eyeSquintRight;
        public float eyeWideRight;
        public float jawForward;
        public float jawLeft;
        public float jawRight;
        public float jawOpen;
        public float mouthClose;
        public float mouthFunnel;
        public float mouthPucker;
        public float mouthLeft;
        public float mouthRight;
        public float mouthSmileLeft;
        public float mouthSmileRight;
        public float mouthFrownLeft;
        public float mouthFrownRight;
        public float mouthDimpleLeft;
        public float mouthDimpleRight;
        public float mouthStretchLeft;
        public float mouthStretchRight;
        public float mouthRollLower;
        public float mouthRollUpper;
        public float mouthShrugLower;
        public float mouthShrugUpper;
        public float mouthPressLeft;
        public float mouthPressRight;
        public float mouthLowerDownLeft;
        public float mouthLowerDownRight;
        public float mouthUpperUpLeft;
        public float mouthUpperUpRight;
        public float browDownLeft;
        public float browDownRight;
        public float browInnerUp;
        public float browOuterUpLeft;
        public float browOuterUpRight;
        public float cheekPuff;
        public float cheekSquintLeft;
        public float cheekSquintRight;
        public float noseSneerLeft;
        public float noseSneerRight;
        public float tongueOut;
    }

    [System.Serializable]
    public struct Vector3Frame
    {
        public float x;
        public float y;
        public float z;

        public override string ToString() => $"VF3: {x}, {y}, {z} :";
    }

    [System.Serializable]
    public struct Vector4Frame
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public override string ToString() => $"VF4: {x}, {y}, {z}, {w} :";
    }

    [System.Serializable]
    public struct ActorJointFrame
    {
        public Vector3Frame position;
        public Vector4Frame rotation;
    }

    [System.Serializable]
    public struct CharacterJointFrame
    {
        public string name;
        public int parent; //!< parent index in a chracter frame joints array (-1 for the root)
        public Vector3Frame position;
        public Vector4Frame rotation;
    }

    #endregion

    public enum PropType
    {
        NORMAL = 0,
        CAMERA = 1,
        STICK = 2
    }
}