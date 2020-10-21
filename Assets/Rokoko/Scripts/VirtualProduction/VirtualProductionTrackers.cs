using UnityEngine;

namespace Rokoko.VirtualProduction
{
    /// <inheritdoc />
    /// <summary>
    /// Makes the game object follow the position and rotation of a tracker. 
    /// </summary>
    public class VirtualProductionTrackers : MonoBehaviour
    {
        public Mesh trackerMesh;
        public Material trackerMaterial;

        // Update is called once per frame
        private void LateUpdate()
        {
            if (VirtualProductionReceiver.Instance == null) return;

            var m = new Matrix4x4[VirtualProductionReceiver.Instance.VirtualProductionData.trackers.Length];
            for (var i = 0; i < VirtualProductionReceiver.Instance.VirtualProductionData.trackers.Length; i++)
            {
                m[i] = Matrix4x4.TRS(VirtualProductionReceiver.Instance.VirtualProductionData.trackers[i].position,
                    VirtualProductionReceiver.Instance.VirtualProductionData.trackers[i].rotation, Vector3.one);
            }

            Graphics.DrawMeshInstanced(trackerMesh, 0, trackerMaterial, m);
        }
    }
}