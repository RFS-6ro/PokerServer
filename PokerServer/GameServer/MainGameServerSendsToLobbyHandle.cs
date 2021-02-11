using System;
using Network;
using PokerSynchronisation;

namespace GameServer
{
	public static class MainGameServerSendsToLobbyHandle
	{
		public enum MainGameServerSendsToLobbyTypes
		{
			LobbyConnectionWelcome = 1,

			PlayerConnection = 2,
			PlayerReadyStateChanged = 3,
			PlayerDisconnection = 4,

			PlayersTurn = 5,
		}

		public static void Welcome(int lobbyId, string message)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToLobbyTypes.LobbyConnectionWelcome))
			{
				packet.Write(lobbyId);
				packet.Write(message);

				MainGameServerSendHandlers.SendTCPData(lobbyId, packet);
			}
		}

		public static void PlayerConnect(int lobbyId, int playerId, string playerName)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToLobbyTypes.PlayerConnection))
			{
				packet.Write(lobbyId);
				packet.Write(playerId);
				packet.Write(playerName);

				MainGameServerSendHandlers.SendTCPData(lobbyId, packet);
			}
		}

		public static void PlayerReadyStateChanged(int lobbyId, int playerId, bool isReady)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToLobbyTypes.PlayerReadyStateChanged))
			{
				packet.Write(lobbyId);
				packet.Write(playerId);
				packet.Write(isReady);

				MainGameServerSendHandlers.SendTCPData(lobbyId, packet);
			}
		}

		public static void PlayerDisconnect(int lobbyId, int playerId)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToLobbyTypes.PlayerDisconnection))
			{
				packet.Write(lobbyId);
				packet.Write(playerId);

				MainGameServerSendHandlers.SendTCPData(lobbyId, packet);
			}
		}

		public static void PlayerTurn(int lobbyId, int playerId, TurnType turn, int raiseAmount)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToLobbyTypes.PlayersTurn))
			{
				packet.Write(lobbyId);
				packet.Write(playerId);
				packet.Write((int)turn);
				packet.Write(raiseAmount);

				MainGameServerSendHandlers.SendTCPData(lobbyId, packet);
			}
		}
	}
}
