using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Rokoko.Core
{
    public class StudioReceiver : UDPReceiver
    {
        public event EventHandler<LiveFrame_v4> onStudioDataReceived;

        protected override void OnDataReceived(byte[] data, IPEndPoint endPoint)
        {
            LiveFrame_v4 liveFrame_V4 = null;
            try
            {
                base.OnDataReceived(data, endPoint);

                // Decompress LZ4
                byte[] uncompressed = LZ4Wrapper.Decompress(data);
                if (uncompressed == null || uncompressed.Length == 0)
                {
                    Debug.LogError("Incoming data are in bad format. Please ensure you are using JSON v3 as forward data format");
                    return;
                }

                // Convert from Json
                string text = System.Text.Encoding.UTF8.GetString(uncompressed);
                liveFrame_V4 = JsonUtility.FromJson<LiveFrame_v4>(text);

                if (liveFrame_V4 == null)
                {
                    Debug.LogError("Incoming data are in bad format. Please ensure you are using JSON v3 as forward data format");
                    return;
                }
            }
            catch { }

            onStudioDataReceived?.Invoke(this, liveFrame_V4);
        }
    }
}