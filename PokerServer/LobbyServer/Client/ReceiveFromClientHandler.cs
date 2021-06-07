using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;

namespace LobbyServer.Client.Handlers
{
	public enum clientTOlobby
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
			Handlers.Add((int)clientTOlobby.Test, Test);
			Handlers.Add((int)clientTOlobby.Connect, Connect);
			Handlers.Add((int)clientTOlobby.Disconnect, Disconnect);
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
