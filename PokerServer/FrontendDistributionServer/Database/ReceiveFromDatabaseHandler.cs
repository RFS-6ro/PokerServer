using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;

namespace FrontendDistributionServer.Database
{
	public enum databaseTOfrontend
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class ReceiveFromDatabaseHandler : ClientReceiveHandlerBase<FrontendDistribution_Database>
	{
		public ReceiveFromDatabaseHandler()
		{
			Handlers.Add((int)databaseTOfrontend.Test, Test);
			Handlers.Add((int)databaseTOfrontend.Connect, Connect);
			Handlers.Add((int)databaseTOfrontend.Disconnect, Disconnect);
		}

		private void Disconnect(UniCastPacket packet)
		{
		}

		private void Connect(UniCastPacket packet)
		{
			Guid senderGuid = new Guid(packet.Read(16));
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
