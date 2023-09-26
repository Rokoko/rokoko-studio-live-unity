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
        public bool useLZ4Compression = true;
        public bool verbose = false;

        protected override void OnDataReceived(byte[] data, IPEndPoint endPoint)
        {
            LiveFrame_v4 liveFrame_V4 = null;
            try
            {
                base.OnDataReceived(data, endPoint);
                byte[] uncompressed;

                if (useLZ4Compression)
                {
                    // Decompress LZ4
                    uncompressed = LZ4Wrapper.Decompress(data);
                    if (uncompressed == null || uncompressed.Length == 0)
                    {
                        Debug.LogError("Incoming data are in bad format. Please ensure you are using JSON v3 as forward data format");
                        return;
                    }    
                }
                else
                {
                    uncompressed = data;
                }

                // Convert from Json
                string text = System.Text.Encoding.UTF8.GetString(uncompressed);
                if (verbose)
                {
                    Debug.Log(text);
                }
                liveFrame_V4 = JsonUtility.FromJson<LiveFrame_v4>(text);

                if (liveFrame_V4 == null)
                {
                    Debug.LogError("Incoming data are in bad format. Please ensure you are using JSON v3 as forward data format");
                    return;
                }

                if (verbose)
                {
                    int numberOfActors = (liveFrame_V4.scene.actors != null) ? liveFrame_V4.scene.actors.Length : 0;
                    int numberOfChars = (liveFrame_V4.scene.characters != null) ? liveFrame_V4.scene.characters.Length : 0;

                    if (numberOfActors == 0 && numberOfChars == 0)
                    {
                        Debug.LogError("Incoming data has no actors and no characters in the stream");
                    }
                }
            }
            catch { }

            onStudioDataReceived?.Invoke(this, liveFrame_V4);
        }
    }
}