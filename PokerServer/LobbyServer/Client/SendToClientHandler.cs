using System;
using UniCastCommonData;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace LobbyServer.Client.Handlers
{
	public enum lobbyTOclient
	{
		None = 0,

		Connect,

		CurrentGameState,
		NewPlayerConnect,

		StartGame,
		StartHand,
		StartRound,
		StartTurn,

		DealCardsToPlayer,
		DealCardsToTable,

		DealerButton,

		UpdatepPot,
		UpdatePlayersMoney,
		UpdateTimer,

		PlayerTurn,
		OpponentCards,

		EndTurn,
		EndRound,
		EndHand,
		EndGame,

		ClearCards,
		PlayerDisconnect,
		Winners,

		Count,


		Test
	}

	public class SendToClientHandler : SessionSender<Lobby_Client_Server>
	{
		public SendToClientHandler()
		{
			Handlers.Add((int)lobbyTOclient.Test, Test);
			Handlers.Add((int)lobbyTOclient.Connect, Connect);
		}

		private void Connect(InitialSendingData data)
		{
			SendAsync(data, null);
		}

		private void Test(InitialSendingData data)
		{
			SendAsync(data, new byte[][]
			{
				GetType().ToString().ToByteArray()
			});
		}
	}
}
