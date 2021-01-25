using System;
using Network;

namespace FrontendServer
{
	public static class ServerHandle
	{
		internal static void WelcomeReceived(int fromClient, Packet packet)
		{
			int clientIdCheck = packet.ReadInt();
			string username = packet.ReadString();

			Console.WriteLine($"{IServer.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}.");
			if (fromClient != clientIdCheck)
			{
				Console.WriteLine($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIdCheck})!");
			}

			PokerSynchronisation.ServerPacketsSend.ConnectToOtherServer(fromClient, ServerPoolHandler.GetClosestServer(fromClient), ServerSendHandlers.SendTCPData);
			//IServer.Clients[fromClient].Disconnect();
		}
	}
}
