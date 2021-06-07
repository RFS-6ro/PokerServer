using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;

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

	public class ReceiveFromDatabaseHandler : ReceiveHandlerBase
	{
		public ReceiveFromDatabaseHandler()
		{
			Handlers.Add((int)databaseTOlobby.Test, Test);
			Handlers.Add((int)databaseTOlobby.Connect, Connect);
			Handlers.Add((int)databaseTOlobby.Disconnect, Disconnect);
		}

		private void Disconnect(UniCastPacket packet)
		{
		}

		private void Connect(UniCastPacket packet)
		{
			Guid senderGuid = new Guid(packet.Read(16));
			Guid receiverGuid = new Guid(packet.Read(16));
			IStaticInstance<Lobby_Database>.Instance.SetId(receiverGuid);
		}

		private void Test(UniCastPacket packet)
		{
			Guid senderGuid = new Guid(packet.Read(16));
			Guid receiverGuid = new Guid(packet.Read(16));

			string message = packet.ReadString();

			Console.WriteLine(senderGuid + "|" + message);
		}
	}
}
