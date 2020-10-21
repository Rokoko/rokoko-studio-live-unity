using System.Net;
using System.Net.Sockets;
using System.Threading;
using Rokoko;
using UnityEngine;
using UnityEngine.Serialization;

namespace Rokoko.VirtualProduction
{
	/// <summary>
	/// Receives virtual production data and parses them into VirtualProductionFrame object.
	/// </summary>
	public class VirtualProductionReceiver : Threadable
	{
		[SerializeField] private int _port = 14045;

		private UdpClient _receiver;
		private Thread _thread;
		private bool _running;

		public VirtualProductionFrame VirtualProductionData;

		public static VirtualProductionReceiver Instance;

		private void OnEnable()
		{
			Instance = this;
			_receiver = new UdpClient(_port);
			_running = true;
			AlwaysListen();
		}

		private void AlwaysListen()
		{
			_thread = new Thread(() =>
			{
				while (_running)
				{
					var endPoint = new IPEndPoint(IPAddress.Any, _port);
					var data = _receiver.Receive(ref endPoint);
					this.AddTaskToMainThread(() => HandleData(data, endPoint));
				}
			}) {IsBackground = true};
			_thread.Start();

		}

		private void HandleData(byte[] data, IPEndPoint endPoint)
		{
			var json = System.Text.Encoding.ASCII.GetString(data);
			VirtualProductionData = JsonUtility.FromJson<VirtualProductionFrame>(json);	
		}

		private void OnDisable()
		{
			_running = false;
			_thread.Abort();
			_receiver.Close();
		}

		private void LateUpdate()
		{
			foreach (var Prop in VirtualProductionData.props)
			{
				Debug.DrawRay(Prop.position, Prop.rotation*Vector3.up*.2f, Color.cyan);
			
			}
			foreach (var Tracker in VirtualProductionData.trackers)
			{
				Debug.DrawRay(Tracker.position, Tracker.rotation*Vector3.right*.1f, Color.red);
				Debug.DrawRay(Tracker.position, Tracker.rotation*Vector3.up*.1f, Color.green);
				Debug.DrawRay(Tracker.position, Tracker.rotation*Vector3.forward*.1f, Color.blue);
			}
		}
	
	}
}
