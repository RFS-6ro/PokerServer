using System;
using Network;

namespace FrontendServer
{
	public static class ServerHandle
	{
		public static void WelcomeReceived(int fromClient, Packet packet)
		{
			int clientIdCheck = packet.ReadInt();
			string username = packet.ReadString();

			Console.WriteLine($"{AbstractServer.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient} with name {username}.");
			if (fromClient != clientIdCheck)
			{
				Console.WriteLine($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIdCheck})!");
			}

			PokerSynchronisation.ServerPacketsSend.ConnectToOtherServer(fromClient, ServerPoolHandler.GetClosestServer(fromClient), ServerSendHandlers.SendTCPData);
			Console.WriteLine($"Player \"{username}\" (ID: {fromClient}) is disconnecting from Frontend server");
			GC.SuppressFinalize(AbstractServer.Clients[fromClient]);
			AbstractServer.Clients[fromClient] = new FrontendClient(fromClient);
		}
	}
}
