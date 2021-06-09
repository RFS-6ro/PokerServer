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
		ReceiveTurn,
		Disconnect,

		Count,


		Test
	}

	public class ReceiveFromClientHandler : SessionReceiveHandlerBase<Lobby_Client_Server>
	{
		public ReceiveFromClientHandler()
		{
			Handlers.Add((int)clientTOlobby.Test, Test);
			Handlers.Add((int)clientTOlobby.Connect, Connect);
			Handlers.Add((int)clientTOlobby.ReceiveTurn, ReceiveTurn);
			Handlers.Add((int)clientTOlobby.Disconnect, Disconnect);
		}

		private void ReceiveTurn(UniCastPacket obj)
		{
		}

		private void Disconnect(UniCastPacket packet)
		{
		}

		private void Connect(UniCastPacket packet)
		{
			Guid receiverGuid = new Guid(packet.Read(16));
			string name = packet.ReadString();

			IStaticInstance<PokerInitializator>.Instance.AddPlayer(LastSenderGuid, name);
		}

		private void Test(UniCastPacket packet)
		{
			Guid receiverGuid = new Guid(packet.Read(16));

			string message = packet.ReadString();

			Console.WriteLine(receiverGuid + "|" + message);
		}
	}
}
