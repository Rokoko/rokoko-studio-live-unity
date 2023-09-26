using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Rokoko.CommandAPI
{
    /// <summary>
    /// This component provides access to Studio's Command API.
    /// </summary>
    public abstract class StudioCommandAPIBase : MonoBehaviour
    {
        [Tooltip("The api key as defined in Studio. Settings->Command API->API key")]
        public string apiKey;

        [Tooltip("The port number as defined in Studio. Settings->Command API->Listen port")]
        public int port;

        [Header("Log extra info")]
        public bool debug;

        protected abstract string IP { get; }

        public bool IsTrackerRequestInProgress = false;

        // legacy only commands 
        [ContextMenu("Unicast")]
        public async void Unicast() =>
            await SendRequest("unicast", GetRequestData().ToJson());

        [ContextMenu("Broadcast")]
        public async void Broadcast() =>
            await SendRequest("broadcast", GetRequestData().ToJson());

        [ContextMenu("Restart Smartsuit")]
        public async void RestartSmartsuit()
            => await SendRequest("Restart", GetRequestData().ToJson());

        // commands that are compatible with Studio and Studio Legacy

        [ContextMenu("Start Recording")]
        public async void StartRecording() =>
            await SendRequest("recording/start", GetRecordingRequestData().ToJson());

        [ContextMenu("Stop Recording")]
        public async void StopRecording() =>
            await SendRequest("recording/stop", GetRecordingRequestData().ToJson());

        [ContextMenu("Calibrate all")]
        public async void CalibrateAll() =>
            await SendRequest("calibrate", GetCalibrateRequestData().ToJson());

        [ContextMenu("Reset Actor")]
        public async void ResetActor()
            => await SendRequest("resetactor", GetResetActorRequestData().ToJson());

        // commands which are not presented in Studio Legacy

        [ContextMenu("Tracker")]
        public async void Tracker() =>
            await SendRequest("tracker", GetTrackerRequestData().ToJson());

        [ContextMenu("Info")]
        public async void Info() =>
            await SendRequest("info", GetInfoRequestData().ToJson());

        [ContextMenu("Playback")]
        public async void PlaybackPlay() =>
            await SendRequest("playback", GetPlaybackRequestData().ToJson());

        [ContextMenu("Livestream")]
        public async void ToggleLiveStream() =>
            await SendRequest("livestream", GetLivestreamRequestData().ToJson());


        private Task<string> SendRequest(string endpoint, string json)
        {
            var tcs = new TaskCompletionSource<string>();
            StartCoroutine(SendRequestEnum(endpoint, json, tcs));
            return tcs.Task;
        }

        protected abstract RequestData GetRequestData();
        protected abstract CalibrateRequestData GetCalibrateRequestData();
        protected abstract ResetActorRequestData GetResetActorRequestData();
        protected abstract RecordingRequestData GetRecordingRequestData();
        protected abstract TrackerRequestData GetTrackerRequestData();
        protected abstract PlaybackRequestData GetPlaybackRequestData();
        protected abstract LivestreamRequestData GetLivestreamRequestData();
        protected abstract InfoRequestData GetInfoRequestData();

        private IEnumerator SendRequestEnum(string endpoint, string json, TaskCompletionSource<string> task)
        {
            IsTrackerRequestInProgress = true;
            
            string url = $"http://{IP}:{port}/v1/{apiKey}/{endpoint}";
            if (debug)
            {
                Debug.Log("Sending request: " + url, this.transform);
                Debug.Log("Sending data: " + json, this.transform);
            }

            // convert json string to byte
            byte[] formData = System.Text.Encoding.UTF8.GetBytes(json);
            UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            UploadHandlerRaw uploadHandler = new UploadHandlerRaw(formData);
            request.uploadHandler = uploadHandler;
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            string body = request.downloadHandler.text;
            if (request.result != UnityWebRequest.Result.ConnectionError)
            {
                if (debug)
                    Debug.Log($"Response: {request.responseCode}: {body}", this.transform);
                OnCommmandResponse(JsonUtility.FromJson<ResponseMessage>(body));
            }
            else
            {
                if (debug)
                    Debug.LogWarning($"There was an error sending request: {request.error}\n{body}", this.transform);
                OnCommmandError(request.error);
            }
            task.SetResult(body);
            request.Dispose();
            uploadHandler.Dispose();
            IsTrackerRequestInProgress = false;
        }

        protected virtual void OnCommmandResponse(ResponseMessage response)
        {

        }

        protected virtual void OnCommmandError(string error)
        {

        }
    }

    [System.Serializable]
    public class ResponseMessage
    {
        public string description;
        public string response_code;
        public long startTime;
        public dynamic[] parameters;
    }
}