using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;

namespace RegionServer.Client.Handlers
{
	public enum clientTOregion
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class ReceiveFromClientHandler : ReceiveHandlerBase
	{
		public ReceiveFromClientHandler()
		{
			Handlers.Add((int)clientTOregion.Test, Test);
			Handlers.Add((int)clientTOregion.Connect, Connect);
			Handlers.Add((int)clientTOregion.Disconnect, Disconnect);
		}

		private void Disconnect(UniCastPacket packet)
		{
		}

		private void Connect(UniCastPacket packet)
		{
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