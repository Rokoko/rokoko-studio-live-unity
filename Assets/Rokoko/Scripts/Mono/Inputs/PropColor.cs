using Rokoko.Core;
using Rokoko.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rokoko.Inputs
{
    public class PropColor : Prop
    {
        [HideInInspector] public MeshRenderer meshRenderer;

        public override void UpdateProp(PropFrame propFrame)
        {
            base.UpdateProp(propFrame);

            if (meshRenderer != null)
                meshRenderer.material.color = propFrame.color.ToColor();
        }
    }
}