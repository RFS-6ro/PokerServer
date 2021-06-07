using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace FrontendDistributionServer.Client.Handlers
{
	public enum clientTOfrontend
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class ReceiveFromClientHandler : IReceivedMessageHandler<int>
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; } = new Dictionary<int, Action<UniCastPacket>>();

		public ReceiveFromClientHandler()
		{
			Handlers.Add((int)clientTOfrontend.Test, Test);
			Handlers.Add((int)clientTOfrontend.Connect, Connect);
			Handlers.Add((int)clientTOfrontend.Disconnect, Disconnect);
		}

		private void Disconnect(UniCastPacket packet)
		{
			ThreadManager.ExecuteOnMainThread(() =>
			{
			});
		}

		private void Connect(UniCastPacket packet)
		{
			ThreadManager.ExecuteOnMainThread(() =>
			{
			});
		}

		private void Test(UniCastPacket packet)
		{
			ThreadManager.ExecuteOnMainThread(() =>
			{
				Guid guid = new Guid(packet.Read(16));

				string message = packet.ReadString();

				Console.WriteLine(guid + "|" + message);
			});
		}
	}
}
