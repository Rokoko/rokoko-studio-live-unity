using Studio.Scripts.Live.Serializers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    public void UpdateProp(PropFrame propFrame)
    {
        this.gameObject.name = propFrame.name;
        this.transform.position = propFrame.position.ToVector3();
        this.transform.rotation = propFrame.rotation.ToQuaternion();
    }
}
