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

	public class ReceiveFromLobbyHandler : ReceiveHandlerBase
	{
		public ReceiveFromLobbyHandler()
		{
			Handlers.Add((int)lobbyTOclient.Test, Test);
			Handlers.Add((int)lobbyTOclient.Connect, Connect);
			Handlers.Add((int)lobbyTOclient.Disconnect, Disconnect);
		}

		private void Disconnect(UniCastPacket packet)
		{
		}

		private void Connect(UniCastPacket packet)
		{
			Guid senderGuid = new Guid(packet.Read(16));
			Guid receiverGuid = new Guid(packet.Read(16));
			IStaticInstance<Client_Lobby>.Instance.SetId(receiverGuid);
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