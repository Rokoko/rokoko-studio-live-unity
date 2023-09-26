using UnityEngine;
using System;

namespace Rokoko.CommandAPI
{
    [Flags]
    public enum CommandAPIPlaybackChange : int
    {
        None = 0,
        IsPlaying = 1,
        CurrentTime = 2,
        GoToFirstFrame = 4,
        GoToLastFrame = 8,
        PlaybackSpeed = 16,
    };


    public class PlaybackRequestData
    {
        [SerializeField] private bool is_playing = false;    // defines the timeline play / pause state
        [SerializeField] private double current_time = 0.0;  // defines a current time on a timeline
        [SerializeField] private float playback_speed = 1.0f;// defines a playback speed multiplier
        [SerializeField] private CommandAPIPlaybackChange change_flag;

        // public members

        public bool IsPlaying { get => is_playing; set => is_playing = value; }
        public double CurrentTime { get => current_time; set => current_time = value; }
        public float PlaybackSpeed { get => playback_speed; set => playback_speed = value; }
        public CommandAPIPlaybackChange ChangeFlag { get => change_flag; set => change_flag = value; }
        

        public override string ToString()
        {
            return $"{IsPlaying}, {CurrentTime}, {PlaybackSpeed}, {ChangeFlag}";
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}