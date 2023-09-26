using UnityEngine;

namespace Rokoko.CommandAPI
{
    public class TrackerRequestData
    {
        // the struct is used to serialize attributes the same way as System.Numberics do
        [System.Serializable]
        private struct TrackerVector3
        {
            [SerializeField] public float X;
            [SerializeField] public float Y;
            [SerializeField] public float Z;

            public TrackerVector3(float x, float y, float z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }
        [System.Serializable]
        private struct TrackerQuaternion
        {
            [SerializeField] public float X;
            [SerializeField] public float Y;
            [SerializeField] public float Z;
            [SerializeField] public float W;
            [SerializeField] public bool IsIdentity;

            public TrackerQuaternion(float x, float y, float z, float w)
            {
                X = x;
                Y = y;
                Z = z;
                W = w;
                IsIdentity = false; 
            }
        }

        // tracker attributes

        [SerializeField] private string device_id = "";
        [SerializeField] private string bone_attached = "";
        [SerializeField] private TrackerVector3 position;
        [SerializeField] private TrackerQuaternion rotation;
        [SerializeField] private float timeout = 2f;
        [SerializeField] private bool is_query_only = false;


        // public members

        public string DeviceId { get => device_id; set => device_id = value; }
        public string BoneAttached { get => bone_attached; set => bone_attached = value; }
        public Vector3 Position { 
            get => new Vector3(position.X, position.Y, position.Z); 
            set => position = new TrackerVector3(value.x, value.y, value.z); }
        public Quaternion Rotation { 
            get => new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W); 
            set => rotation = new TrackerQuaternion(value.x, value.y, value.z, value.w); }
        public float Timeout { get => timeout; set => timeout = value; }
        public bool IsQueryOnly { get => is_query_only; set => is_query_only = value; }


        public override string ToString()
        {
            return $"{DeviceId}, {BoneAttached}, {Position}, {Rotation}, {Timeout}";
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}