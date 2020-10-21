using UnityEngine;

namespace Rokoko.CommandAPI
{
    public class RequestData
    {
        public string smartsuit = "";
        public float countdown_delay = 4;
        public string filename = "";

        public override string ToString()
        {
            return smartsuit + "," + countdown_delay + ", " + filename;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}