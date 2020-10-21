using Rokoko.Serializers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FrameHelper
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
        if (color.Length != 3)
            return Color.white;
        return new Color((float)color[0] / 255f, (float)color[1] / 255f, (float)color[2] / 255f);
    }
}
