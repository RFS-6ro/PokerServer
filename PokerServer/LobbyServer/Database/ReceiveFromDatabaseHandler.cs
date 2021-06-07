using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace FrontendDistributionServer.Database
{
	public enum databaseTOlobby
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class ReceiveFromDatabaseHandler : IReceivedMessageHandler<int>
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; } = new Dictionary<int, Action<UniCastPacket>>();

		public ReceiveFromDatabaseHandler()
		{
			Handlers.Add((int)databaseTOlobby.Test, Test);
			Handlers.Add((int)databaseTOlobby.Connect, Connect);
			Handlers.Add((int)databaseTOlobby.Disconnect, Disconnect);
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
				Guid senderGuid = new Guid(packet.Read(16));
				Guid receiverGuid = new Guid(packet.Read(16));
				IStaticInstance<Lobby_Database>.Instance.SetId(receiverGuid);
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
