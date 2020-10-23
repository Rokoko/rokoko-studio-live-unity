using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Rokoko.Core
{
    public class UDPReceiver
    {
        public int sendPortNumber = 14043;
        public int receivePortNumber = 14043;
        public int bufferSize = 65000;

        private UdpClient client;
        private Thread thread;

        public virtual void Initialize()
        {
            try
            {
                client = new UdpClient(receivePortNumber);
                client.Client.SendBufferSize = bufferSize;
            }
            catch (SocketException)
            {
                Debug.LogError($"Seem like port:{receivePortNumber} is already in use. Is plugin running already in other application?");
            }
            catch(System.Exception ex)
            {
                throw ex;
            }
        }

        public virtual void StartListening()
        {
            if (client == null)
            {
                Debug.LogError("UDPReceiver - Client isn't initialized.");
                return;
            }

            if (thread != null)
            {
                Debug.LogWarning("UDPReceiver - Cannot start listening. Thread is already listening");
                return;
            }

            StartListeningThread();
        }

        public virtual void StopListening()
        {
            thread?.Abort();
            client?.Close();
        }

        public virtual void Dispose()
        {
            StopListening();
            client?.Dispose();
            client = null;
        }

        public void Send(string ipAddress, byte[] data)
        {
            Send(ipAddress, data, sendPortNumber);
        }

        public void Send(string ipAddress, byte[] data, int portNumber)
        {
            client?.Send(data, data.Length, new IPEndPoint(IPAddress.Parse(ipAddress), portNumber));
        }

        public void Send(IPEndPoint endPoint, byte[] data)
        {
            client?.Send(data, data.Length, endPoint);
        }

        public bool IsListening() => thread != null;

        protected virtual void OnDataReceived(byte[] data, IPEndPoint endPoint) { }

        private void StartListeningThread()
        {
            thread = new Thread(ListenToUDP);
            thread.IsBackground = true;
            thread.Start();
        }

        private void ListenToUDP()
        {
            while (client != null)
            {
                try
                {
                    IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, receivePortNumber);
                    byte[] data = client.Receive(ref endpoint);
                    OnDataReceived(data, endpoint);
                }
                catch (ThreadAbortException) { }
                catch (SocketException) { }
                catch (System.Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }
    }
}
