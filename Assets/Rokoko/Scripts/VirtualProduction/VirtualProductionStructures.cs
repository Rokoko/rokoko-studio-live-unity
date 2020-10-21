using System.Collections.Generic;
using UnityEngine;

namespace Rokoko.VirtualProduction
{
    [System.Serializable]
    public class VirtualProductionFrame
    {
        public int version;
        public Prop[] props;
        public Tracker[] trackers;
        public FaceData[] faces;
    }

    [System.Serializable]
    public class Prop
    {
        public string name;
        public string id;
        public Vector3 position;
        public Quaternion rotation;
        public bool isLive;
        public PropProfile profile;
    }


    [System.Serializable]
    public enum VrPropType
    {
        NORMAL = 0,
        CAMERA = 1,
        STICK = 2
    }

    [System.Serializable]
    public class PropProfile
    {
        public string name;
        public string uuid;

        public Vector3 dimensions;

        public Vector3 color;
        public ReferencePoint trackerOffset;
        public ReferencePoint pivot;
        public List<RadiusReferencePoint> grips;

        public VrPropType propType;
    }

    [System.Serializable]
    public struct ReferencePoint
    {
        public Vector3 position;
        public Vector3 rotation;
    }

    [System.Serializable]
    public struct RadiusReferencePoint
    {
        public float radius;
        public Vector3 position;
        public Vector3 rotation;
    }

    [System.Serializable]
    public enum ETrackingResult
    {
        Uninitialized = 1,
        Calibrating_InProgress = 100,
        Calibrating_OutOfRange = 101,
        Running_OK = 200,
        Running_OutOfRange = 201,
    }


    [System.Serializable]
    public enum VRTrackerType
    {
        INVALID = -1,
        HEAD = 0,
        TRACKER = 1,
        CONTROLLER = 2,
        BASESTATION = 3
    }

    [System.Serializable]
    public class Tracker
    {
        public string name;
        public string connectionId;
        public Vector3 position;
        public Quaternion rotation;
        public bool isLive;

        public ETrackingResult trackingResult;
        public VRTrackerType trackerType;
        public string RenderModelName;
        public float battery;
    }

    [System.Serializable]
    public class FaceData
    {
        public string type;
        public int version; // 4
        public float timestamp; // 4
        public string provider; // 20
        public string faceId; // 20
        public float eyeBlinkLeft; // 4
        public float eyeLookDownLeft; // 4
        public float eyeLookInLeft; // 4
        public float eyeLookOutLeft; // 4
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
}