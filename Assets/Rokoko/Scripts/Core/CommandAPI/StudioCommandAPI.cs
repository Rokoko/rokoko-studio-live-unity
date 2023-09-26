using UnityEngine;
using UnityEngine.UI;

namespace Rokoko.CommandAPI
{
    public class StudioCommandAPI : StudioCommandAPIBase
    {
        [Header("Show CommandAPI response (Optional)")]
        [SerializeField] private Text responseText = null;

        [Header("The IP address of Studio. Leave default for same machine")]
        public string ipAddress = "127.0.0.1";

        [Header("Common Parameters")]        
        [Tooltip("The actor name or hardware device id in the scene")]
        [SerializeField] public string deviceId;

        [Header("Calibration Parameters")]
        [Tooltip("The calibration countdown delay")]
        [SerializeField] public int countDownDelay = 3;

        [Tooltip("The calibration skip suit")]
        [SerializeField] public bool calibrationSkipSuit = false;

        [Tooltip("The calibration skip gloves")]
        [SerializeField] public bool calibrationSkipGloves = false;

        [Header("Recording Parameters")]
        [Tooltip("Recording Clip Name")]
        [SerializeField] public string clipName = "NewClip";

        [Tooltip("Recording Start / Stop Time (SMPTE)")]
        [SerializeField] public string recordingTime = "00:00:00:00";

        [Tooltip("Return To Live Mode When Recording Is Done")]
        [SerializeField] public bool backToLive = false;

        [Header("Tracker Parameters")]
        [SerializeField] public string BoneAttached = "";
        [SerializeField] public float TrackerTimeout = 2f;
        [SerializeField] public bool IsQueryOnly = false;

        [SerializeField] public Transform TrackerTransform;

        [Header("Playback Parameters")]
        [SerializeField] public bool IsPlaying = true;
        
        [Header("Livestream Parameters")]
        [SerializeField] public bool IsStreaming = true;

        [Header("Info Parameters")]
        [SerializeField] public bool RequestDevicesInfo = true;
        [SerializeField] public bool RequestClipsInfo = true;
        [SerializeField] public bool RequestActorsInfo = true;
        [SerializeField] public bool RequestCharactersInfo = true;

        protected override string IP => ipAddress;
        protected override RequestData GetRequestData() => new RequestData();
        protected override ResetActorRequestData GetResetActorRequestData() 
        {
            var data = new ResetActorRequestData() {DeviceId = deviceId};
            if (debug)
            {
                Debug.Log(data.ToJson());    
            }
            return data;
        }

        protected override CalibrateRequestData GetCalibrateRequestData() 
        {
            var data = new CalibrateRequestData() 
            {
                DeviceId = deviceId, 
                CountDownDelay = countDownDelay,
                SkipSuit = calibrationSkipSuit,
                SkipGloves = calibrationSkipGloves
            };
            if (debug)
            {
                Debug.Log(data.ToJson());    
            }
            return data;
        }

        protected override RecordingRequestData GetRecordingRequestData() 
        {
            var data = new RecordingRequestData() 
            {
                Filename = clipName, 
                Time = recordingTime,
                BackToLive = backToLive
            };
            if (debug)
            {
                Debug.Log(data.ToJson());    
            }
            return data;
        }

        protected override TrackerRequestData GetTrackerRequestData()
        {
            var data = new TrackerRequestData()
            {
                DeviceId = deviceId,
                BoneAttached = BoneAttached,
                Position = TrackerTransform?.position ?? Vector3.zero,
                Rotation = TrackerTransform?.rotation ?? Quaternion.identity,
                Timeout = TrackerTimeout,
                IsQueryOnly = IsQueryOnly
            };
            if (debug)
            {
                Debug.Log(data.ToJson());    
            }
            return data;
        }

        protected override PlaybackRequestData GetPlaybackRequestData()
        {
            var data = new PlaybackRequestData()
            {
                IsPlaying = IsPlaying,
                CurrentTime = 0.0,
                PlaybackSpeed = 1.0f,
                ChangeFlag = CommandAPIPlaybackChange.IsPlaying
            };
            if (debug)
            {
                Debug.Log(data.ToJson());    
            }
            return data;
        }

        protected override LivestreamRequestData GetLivestreamRequestData()
        {
            var data = new LivestreamRequestData()
            {
                Enabled = IsStreaming
            };
            if (debug)
            {
                Debug.Log(data.ToJson());    
            }
            return data;
        }

        protected override InfoRequestData GetInfoRequestData()
        {
            var data = new InfoRequestData()
            {
                DoDevicesInfo = RequestDevicesInfo,
                DoClipsInfo = RequestClipsInfo,
                DoActorsInfo = RequestActorsInfo,
                DoCharactersInfo = RequestCharactersInfo
            };
            if (debug)
            {
                Debug.Log(data.ToJson());    
            }
            return data;
        }

        private void Start()
        {
            SetStatusText("");
        }

        protected override void OnCommmandResponse(ResponseMessage response)
        {
            base.OnCommmandResponse(response);
            SetStatusText(response.description);
        }

        protected override void OnCommmandError(string error)
        {
            base.OnCommmandError(error);
            SetStatusText($"{error}\nPlease make sure Rokoko Studio is running and Command API is enabled (Menu->Settings->Command API->Enabled).\nCheck also the receiving port and API key in both Rokoko Studio and Unity plugin.");
        }

        private void SetStatusText(string text)
        {
            if (responseText == null) return;
            responseText.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(text));
            responseText.text = text;
            LayoutRebuilder.ForceRebuildLayoutImmediate(responseText.transform.parent as RectTransform);
        }
    }
}