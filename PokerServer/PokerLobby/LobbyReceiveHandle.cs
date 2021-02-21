using System;
using System.Linq;
using Network;
using PokerSynchronisation;

namespace PokerLobby
{
	public static class LobbyReceiveHandle
	{
		public enum LobbyReceiveTypes
		{
			LobbyConnectionWelcome = 1,

			PlayerConnection = 2,
			PlayerReadyStateChanged = 3,
			PlayerDisconnection = 4,

			PlayersTurn = 5,
		}

		public static void LobbyConnectionWelcome(Packet packet)
		{
			int lobbyId = packet.ReadInt();
			string message = packet.ReadString();

			LobbyClient.Instance.Id = lobbyId;
			ConsoleLogger.Instance.Print($"Message from server: {message}");
			LobbySendHandle.WelcomeReceived(lobbyId, "Lobby success launch");
		}

		public static void PlayerConnect(Packet packet)
		{
			int lobbyId = packet.ReadInt();

			//Check LobbyId
			if (lobbyId != LobbyClient.Instance.Id)
			{
				return;
			}

			int playerId = packet.ReadInt();
			string name = packet.ReadString();
			ConsoleLogger.Instance.Print($"Player ID:{playerId} {name} is connecting");

			if (LobbyClient.Instance.TryConnectPlayer(playerId, name, out int serverSideIndex))
			{
				LobbySendHandle.ConnectionToLobbyApprovance(lobbyId, playerId, serverSideIndex);
			}
			else
			{
				LobbySendHandle.ConnectionToLobbyApprovance(lobbyId, playerId, "Lobby is not avaliable");
			}
		}

		public static void PlayerReadyStateChanged(Packet packet)
		{
			int lobbyId = packet.ReadInt();

			//Check LobbyId
			if (lobbyId != LobbyClient.Instance.Id)
			{
				return;
			}

			int playerId = packet.ReadInt();
			bool isReady = packet.ReadBool();


			ConsoleLogger.Instance.Print($"Player {playerId} is {(isReady ? "ready" : "not ready")}.");
			LobbyClient.Instance.SetReadyState(playerId, isReady);
		}

		public static void PlayerDisconnect(Packet packet)
		{
			int lobbyId = packet.ReadInt();

			//Check LobbyId
			if (lobbyId != LobbyClient.Instance.Id)
			{
				return;
			}

			int playerId = packet.ReadInt();
			ConsoleLogger.Instance.Print($"Player {playerId} is disconnecting");

			LobbyClient.Instance.DisconnectPlayer(playerId);
		}

		public static void PlayerTurn(Packet packet)
		{
			int lobbyId = packet.ReadInt();

			//Check LobbyId
			if (lobbyId != LobbyClient.Instance.Id)
			{
				return;
			}

			int playerId = packet.ReadInt();
			TurnType turn = (TurnType)packet.ReadInt();
			int raiseAmount = packet.ReadInt();

			ConsoleLogger.Instance.Print($"Player {playerId} turn is {turn} with raise amount {raiseAmount}");
			RealPlayer turningPlayer = LobbyClient.Players.FirstOrDefault((x) => x.ServerId == playerId);

			if (turningPlayer != null)
			{
				//CHECK: Send turn event to player;
				turningPlayer.ApplyExternalInput(turn, raiseAmount);
			}
		}
	}
}