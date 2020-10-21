using System.Linq;
using UnityEngine;

namespace Rokoko.VirtualProduction
{
    /// <inheritdoc />
    /// <summary>
    /// Makes the game object follow the position and rotation of a tracker. 
    /// </summary>
    public class VirtualProductionTracker : MonoBehaviour
    {
        public string trackerId = "1";
        public bool followLiveTracker = true;

        private Transform _transform;

        private void Start() => _transform = transform;

        // Update is called once per frame
        private void Update()
        {
            foreach(var tracker in VirtualProductionReceiver.Instance.VirtualProductionData.trackers)
            {
                if(tracker.isLive == followLiveTracker && tracker.name == trackerId)
                {
                    _transform.position = tracker.position;
                    _transform.rotation = tracker.rotation;
                    break;
                }
            }
        }
    }
}