using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;
using UniCastCommonData.Network.MessageHandlers;

namespace TestingClient.Lobby.Handlers
{
	public enum clientTOlobby
	{
		None = 0,

		Connect,
		SendTurn,
		Disconnect,

		Count,


		Test
	}

	public class SendToLobbyHandler : ISendMessageHandler<int>
	{
		public ISender Sender { get; set; }

		public Dictionary<int, Action<InitialSendingData>> Handlers { get; } = new Dictionary<int, Action<InitialSendingData>>();

		public SendToLobbyHandler()
		{
			Handlers.Add((int)clientTOlobby.Test, Test);
			Handlers.Add((int)clientTOlobby.Connect, Connect);
			Handlers.Add((int)clientTOlobby.SendTurn, SendTurn);
			Handlers.Add((int)clientTOlobby.Disconnect, Disconnect);
		}

		private void Connect(InitialSendingData data)
		{
			using (UniCastPacket packet = new UniCastPacket(data))
			{
				packet.Write("Name");
				Sender.SendAsync(packet);
			}
		}

		private void SendTurn(InitialSendingData data)
		{
			using (UniCastPacket packet = new UniCastPacket(data))
			{
				packet.Write(InputModel.GetTurn());
				Sender.SendAsync(packet);
			}
		}

		private void Disconnect(InitialSendingData data)
		{
			using (UniCastPacket packet = new UniCastPacket(data))
			{

				Sender.SendAsync(packet);
			}
		}



		private void Test(InitialSendingData data)
		{
			using (UniCastPacket packet = new UniCastPacket(data))
			{
				packet.Write(GetType().ToString());

				Sender.SendAsync(packet);
			}
		}
	}
}