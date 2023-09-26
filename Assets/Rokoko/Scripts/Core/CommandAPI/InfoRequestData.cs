using UnityEngine;

namespace Rokoko.CommandAPI
{
    public class InfoRequestData
    {
        [SerializeField] private bool devices_info = true;    // return a list of scene input devices names
        [SerializeField] private bool clips_info = true; // return a list of scene clip names
        [SerializeField] private bool actors_info = false; // return a list of scene actor names
        [SerializeField] private bool characters_info = false; // return a list of scene character names

        // public members

        public bool DoDevicesInfo { get => devices_info; set => devices_info = value; }
        public bool DoClipsInfo { get => clips_info; set => clips_info = value; }
        public bool DoActorsInfo { get => actors_info; set => actors_info = value; }
        public bool DoCharactersInfo { get => characters_info; set => characters_info = value; }


        public override string ToString()
        {
            return $"{DoDevicesInfo}, {DoClipsInfo}, {DoActorsInfo}, {DoCharactersInfo}";
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}