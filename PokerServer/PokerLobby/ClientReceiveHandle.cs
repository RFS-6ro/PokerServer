using System;
using System.Linq;
using Network;
using PokerSynchronisation;

namespace PokerLobby
{
	public class ClientReceiveHandle
	{
		public static void Welcome(Packet packet)
		{
			int lobbyId = packet.ReadInt();
			string message = packet.ReadString();

			ConsoleLogger.Instance.Print($"Message from server: {message}");
			LobbyClient.Instance.Id = lobbyId;
			LobbySends.WelcomeReceived(lobbyId, "lobby was launched successfully", ClientSentHandlers.SendTCPData);
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

			if (LobbyClient.Instance.TryConnectPlayer(playerId, name))
			{
				LobbySends.ConnectionToLobbyApprovance(lobbyId, playerId, ClientSentHandlers.SendTCPData);
			}
			else
			{
				LobbySends.ConnectionToLobbyApprovance(lobbyId, "Lobby is not responding to you", ClientSentHandlers.SendTCPData);
			}
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

			RealPlayer turningPlayer = LobbyClient.Players.FirstOrDefault((x) => x.ServerId == playerId);

			if (turningPlayer != null)
			{
				//TODO: Send turn event to player;	
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

			LobbyClient.Instance.SetReadyState(playerId, isReady);
		}
	}
}