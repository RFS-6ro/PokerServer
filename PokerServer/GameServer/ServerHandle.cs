using System;
using Network;

namespace GameServer
{
	public class ServerHandle
	{
		public static void WelcomeReceived(int fromClient, Packet packet)
		{
			int clientIdCheck = packet.ReadInt();
			string username = packet.ReadString();

			Console.WriteLine($"{IServer.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}.");
			if (fromClient != clientIdCheck)
			{
				Console.WriteLine($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIdCheck})!");
			}
			//PokerSynchronisation.ServerPacketsSend.ConnectToOtherServer(fromClient, new ServerIdentifierData(NetworkSettings.LOCAL_HOST_IP, NetworkSettings.PORT), ServerSendHandlers.SendTCPData);
		}
	}
}