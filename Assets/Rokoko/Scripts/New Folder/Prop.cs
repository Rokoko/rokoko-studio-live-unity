using Rokoko.Serializers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = this.GetComponent<MeshRenderer>();
    }

    public void UpdateProp(PropFrame propFrame)
    {
        this.gameObject.name = propFrame.name;
        this.transform.position = propFrame.position.ToVector3();
        this.transform.rotation = propFrame.rotation.ToQuaternion();

        if (meshRenderer != null)
            meshRenderer.material.color = propFrame.color.ToColor();
    }
}
