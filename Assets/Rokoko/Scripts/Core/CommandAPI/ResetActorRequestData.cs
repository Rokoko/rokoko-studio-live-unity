using UnityEngine;

namespace Rokoko.CommandAPI
{
    public class ResetActorRequestData
    {
        [SerializeField] private string device_id = "";

        public string DeviceId { get => device_id; set => device_id = value; }
        
        public override string ToString()
        {
            return DeviceId;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}