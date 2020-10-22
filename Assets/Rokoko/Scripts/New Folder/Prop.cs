using Rokoko.Serializers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rokoko
{
    public class Prop : MonoBehaviour
    {
        public string propName { get; private set; }

        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = this.GetComponent<MeshRenderer>();
        }

        public void UpdateProp(PropFrame propFrame)
        {
            propName = propFrame.name;
            this.gameObject.name = propName;
            this.transform.position = propFrame.position.ToVector3();
            this.transform.rotation = propFrame.rotation.ToQuaternion();

            if (meshRenderer != null)
                meshRenderer.material.color = propFrame.color.ToColor();
        }
    }
}