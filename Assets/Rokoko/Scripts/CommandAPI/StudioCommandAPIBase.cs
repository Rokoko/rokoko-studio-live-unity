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

        [Space(10)] public bool debug;

        protected abstract string IP { get; }

        
        [ContextMenu("Restart Smartsuit")]
        public async void RestartSmartsuit()
            => await SendRequest("Restart", GetRequestData().ToJson());

        [ContextMenu("Start Recording")]
        public async void StartRecording() =>
            await SendRequest("recording/start", new RequestData { }.ToJson());

        [ContextMenu("Stop Recording")]
        public async void StopRecording() =>
            await SendRequest("recording/stop", new RequestData { }.ToJson());

        [ContextMenu("Calibrate all")]
        public async void CalibrateAll() =>
            await SendRequest("calibrate", GetRequestData().ToJson());

        [ContextMenu("Unicast")]
        public async void Unicast() =>
            await SendRequest("unicast", GetRequestData().ToJson());

        [ContextMenu("Broadcast")]
        public async void Broadcast() =>
            await SendRequest("broadcast", GetRequestData().ToJson());

        private Task<string> SendRequest(string endpoint, string json)
        {
            var tcs = new TaskCompletionSource<string>();
            StartCoroutine(SendRequestEnum(endpoint, json, tcs));
            return tcs.Task;
        }

        protected abstract RequestData GetRequestData();

        private IEnumerator SendRequestEnum(string endpoint, string json, TaskCompletionSource<string> task)
        {
            Dictionary<string, string> postHeader = new Dictionary<string, string>();
            postHeader.Add("Content-Type", "application/json");
            string url = $"http://{IP}:{port}/v1/{apiKey}/{endpoint}";
            if (debug)
            {
                Debug.Log("Sending request: " + url);
                Debug.Log("Sending data: " + json);
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
            if (request.isNetworkError)
            {
                if (debug)
                    Debug.LogWarning($"There was an error sending request: {request.error}\n{body}");
                OnCommmandError(request.error);
            }
            else
            {
                if (debug)
                    Debug.Log($"Response: {request.responseCode}: {body}");
                OnCommmandResponse(JsonUtility.FromJson<ResponseMessage>(body));
            }
            task.SetResult(body);
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
    }
}