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
		StartRound,
		StartTurn,

		DealCardsToPlayer,
		DealCardsToTable,

		DealerButton,

		Updatepot,
		UpdatePlayersMoney,
		UpdateTimer,

		PlayerTurn,
		OpponentCards,

		EndTurn,
		EndRound,
		EndGame,

		PlayerDisconnect,

		Disconnect,

		Count,


		Test
	}

	public class SendToClientHandler : SessionSender<Lobby_Client_Server>
	{
		public SendToClientHandler()
		{
			Handlers.Add((int)lobbyTOclient.Test, Test);
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
