using Rokoko.Core;
using Rokoko.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rokoko.Inputs
{
    public class Prop : MonoBehaviour
    {
        [HideInInspector] public string propName;
        public Space positionSpace = Space.Self;
        public Space rotationSpace = Space.Self;

        protected virtual void Start()
        {
            if (!string.IsNullOrEmpty(propName))
                StudioManager.AddPropOverride(this);
        }

        public virtual void UpdateProp(PropFrame propFrame)
        {
            propName = propFrame.name;

            if (positionSpace == Space.World)
                this.transform.position = propFrame.position.ToVector3();
            else
                this.transform.localPosition = propFrame.position.ToVector3();

            Quaternion worldRotation = propFrame.rotation.ToQuaternion();
            if (rotationSpace == Space.World)
                this.transform.rotation = worldRotation;
            else
            {
                if (transform.parent != null)
                    this.transform.rotation = Quaternion.Inverse(transform.parent.rotation) * worldRotation;
                else
                    this.transform.rotation = worldRotation;
            }
        }
    }
}