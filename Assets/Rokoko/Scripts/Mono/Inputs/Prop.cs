using Rokoko.Core;
using Rokoko.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rokoko.Inputs
{
    public class Prop : MonoBehaviour
    {
        public string propName { get; private set; }
        public Space positionSpace = Space.Self;
        public Space rotationSpace = Space.Self;

        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = this.GetComponent<MeshRenderer>();
        }

        public void UpdateProp(PropFrame propFrame)
        {
            propName = propFrame.name;
            this.gameObject.name = propName;

            if (positionSpace == Space.World)
                this.transform.position = propFrame.position.ToVector3();
            else
                this.transform.localPosition = propFrame.position.ToVector3();

            Quaternion worldRotation = propFrame.rotation.ToQuaternion();
            if (rotationSpace == Space.World)
                this.transform.rotation = worldRotation;
            else
                this.transform.rotation = Quaternion.Inverse(transform.parent.rotation) * worldRotation;

            if (meshRenderer != null)
                meshRenderer.material.color = propFrame.color.ToColor();
        }
    }
}