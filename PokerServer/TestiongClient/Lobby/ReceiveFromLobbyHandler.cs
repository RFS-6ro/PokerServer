using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;

namespace TestingClient.Lobby.Handlers
{
	public enum lobbyTOclient
	{
		None = 0,

		Connect,
		Disconnect,
		Count,


		Test
	}

	public class ReceiveFromLobbyHandler : ClientReceiveHandlerBase<Client_Lobby>
	{
		public ReceiveFromLobbyHandler()
		{
			Handlers.Add((int)lobbyTOclient.Test, Test);
			Handlers.Add((int)lobbyTOclient.Connect, Connect);
			Handlers.Add((int)lobbyTOclient.Disconnect, Disconnect);
		}

		private void Disconnect(UniCastPacket packet)
		{
			Guid receiverGuid = new Guid(packet.Read(16));
			if (receiverGuid != ReceiverId)
			{
				return;
			}
		}

		private void Connect(UniCastPacket packet)
		{
			Guid receiverGuid = new Guid(packet.Read(16));
			Client.SetId(ReceiverId);
		}

		private void Test(UniCastPacket packet)
		{
			Guid receiverGuid = new Guid(packet.Read(16));
			string message = packet.ReadString();
		}
	}
}