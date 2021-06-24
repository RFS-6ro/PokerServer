using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

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

		private void ReceiveTurn(UniCastPacket packet)
		{
			PlayerInputSendingData sendingData = new PlayerInputSendingData(packet.GetRawBytes());

			PokerInitializator.Instance.FindPlayerDecoratorByGuid(sendingData.SenderGuid)?.SetPlayerTurn(sendingData.InputType, sendingData.InputAmount);
			//RECEIVE INPUT
		}

		private void Disconnect(UniCastPacket packet)
		{
			DisconnectSendingData sendingData = new DisconnectSendingData(packet.GetRawBytes());
			//DISCONNECT
			PokerInitializator.Instance.RemovePlayer(sendingData.Player);
		}

		private void Connect(UniCastPacket packet)
		{
			packet.Read(32);
			string name = packet.ReadString();

			PokerInitializator.Instance.AddPlayer(LastSenderGuid, name);
		}

		private void Test(UniCastPacket packet)
		{
			Guid receiverGuid = new Guid(packet.Read(16));

			string message = packet.ReadString();

			Console.WriteLine(receiverGuid + "|" + message);
		}
	}
}
