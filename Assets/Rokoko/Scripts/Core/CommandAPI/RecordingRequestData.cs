using UnityEngine;

namespace Rokoko.CommandAPI
{
    public class RecordingRequestData
    {
        [SerializeField] private string filename = "";    // actor clip name or filename for recording
        [SerializeField] private string time = "00:00:00:00"; // recording start/stop time in SMPTE format
        [SerializeField] private float frame_rate = 30.0f;
        [SerializeField] private bool back_to_live = false;

        // public members

        public string Filename { get => filename; set => filename = value; }
        public string Time { get => time; set => time = value; }
        public float FrameRate { get => frame_rate; set => frame_rate = value; }
        public bool BackToLive { get => back_to_live; set => back_to_live = value; }
        

        public override string ToString()
        {
            return $"{Filename}, {Time}, {FrameRate}, {BackToLive}";
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}