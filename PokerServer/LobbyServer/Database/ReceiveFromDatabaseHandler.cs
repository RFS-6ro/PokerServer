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

	public class ReceiveFromDatabaseHandler : ClientReceiveHandlerBase<Lobby_Database>
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
			Guid receiverGuid = new Guid(packet.Read(16));
			Client.SetId(receiverGuid);
		}

		private void Test(UniCastPacket packet)
		{
			Guid receiverGuid = new Guid(packet.Read(16));

			string message = packet.ReadString();

			Console.WriteLine(receiverGuid + "|" + message);
		}
	}
}
