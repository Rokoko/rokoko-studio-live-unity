using UnityEngine;

namespace Rokoko.CommandAPI
{
    public class LivestreamRequestData
    {
        [SerializeField] private bool enabled = true;    // control a state of a custom live stream target
        
        // public members

        public bool Enabled { get => enabled; set => enabled = value; }
        
        public override string ToString()
        {
            return $"{Enabled}";
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}