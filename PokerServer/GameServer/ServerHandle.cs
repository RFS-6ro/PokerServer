using System;
using Network;
using PokerSynchronisation;

namespace GameServer
{
	public class ServerHandle
	{
		public static void WelcomeReceived(int fromClient, Packet packet)
		{
			int clientIdCheck = packet.ReadInt();
			string username = packet.ReadString();
			((PokerClient)IServer.Clients[clientIdCheck]).UserName = username;

			Console.WriteLine($"{ IServer.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint } connected successfully and is now player { fromClient } with name { username }.");
			if (fromClient != clientIdCheck)
			{
				Console.WriteLine($"Player \"{ username }\" (ID: { fromClient }) has assumed the wrong client ID ({ clientIdCheck })!");
			}
		}

		public static void ChoseLobby(int fromClient, Packet packet)
		{
			int clientIdCheck = packet.ReadInt();
			string lobbyName = packet.ReadString();

			Console.WriteLine($"{ IServer.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint } connected successfully and trying to join lobby \"{ lobbyName }\".");

			if (fromClient != clientIdCheck)
			{
				Console.WriteLine($"Player (ID: { fromClient }) has assumed the wrong client ID ({ clientIdCheck })!");
			}

			MainGameServer.Instance.ConnectToLobby(clientIdCheck, lobbyName);
		}

		public static void TurnReceive(int fromClient, Packet packet)
		{
			int playerId = packet.ReadInt();
			TurnType turnType = (TurnType)packet.ReadInt();
			int amount = packet.ReadInt();

			Console.WriteLine($"{ IServer.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint } is connected successfully and trying to perform turn: \"{ turnType }\", with amount: { amount }.");

			if (fromClient != playerId)
			{
				Console.WriteLine($"Player (ID: { fromClient }) has assumed the wrong client ID ({ playerId })!");
				ServerPacketsSend.Approvance(playerId, false, ServerSendHandlers.SendTCPData);
				return;
			}

			bool turnAvaliability = false;

			//TODO: Check turn avaliability

			ServerPacketsSend.Approvance(playerId, turnAvaliability, ServerSendHandlers.SendTCPData);
		}

		public static void ExitLobby(int fromClient, Packet packet)
		{
			int clientIdCheck = packet.ReadInt();
			string lobbyName = packet.ReadString();

			Console.WriteLine($"{ IServer.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint } is connected successfully and leaving from lobby \"{ lobbyName }\".");

			if (fromClient != clientIdCheck)
			{
				Console.WriteLine($"Player (ID: { fromClient }) has assumed the wrong client ID ({ clientIdCheck })!");
			}

			MainGameServer.Instance.ExitLobby(clientIdCheck, lobbyName);
			ServerSendHandlers.SendTCPData(fromClient, packet);
		}
	}
}
