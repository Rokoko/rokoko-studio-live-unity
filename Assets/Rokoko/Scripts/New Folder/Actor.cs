using Rokoko.Serializers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Actor : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer meshRenderer = null;

    private Animator animator;
    private Dictionary<HumanBodyBones, Transform> humanBones = new Dictionary<HumanBodyBones, Transform>();

    // Start is called before the first frame update
    private void Awake()
    {
        animator = this.GetComponent<Animator>();
        InitializeBodyBones();

        string txt = "";

        foreach (HumanBodyBones bone in System.Enum.GetValues(typeof(HumanBodyBones)))
        {
            //txt += $"UpdateBone(HumanBodyBones.{bone}, frame.{bone.ToString().ToLowerFirstChar()});\n";
            txt += $"case HumanBodyBones.{bone}:\n return frame.{bone.ToString().ToLowerFirstChar()};\n";
        }
        //System.IO.File.WriteAllText(@"C:\Users\Painkiller\Desktop\Export\bones.txt", txt);
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

    public void UpdateActor(ActorFrame actorFrame)
    {
        this.gameObject.name = actorFrame.name;
        UpdateSkeleton(actorFrame.body);

        if (meshRenderer != null)
            meshRenderer.material.color = actorFrame.color.ToColor();
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
