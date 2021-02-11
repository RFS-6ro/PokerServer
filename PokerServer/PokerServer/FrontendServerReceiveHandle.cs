using System;
using Network;

namespace FrontendServer
{
	public static class FrontendServerReceiveHandle
	{
		public enum FrontendServerReceivedPacketsType
		{
			WelcomeReceived = 1,
		}

		public static void WelcomeReceived(int fromClient, Packet packet)
		{
			int clientIdCheck = packet.ReadInt();
			string username = packet.ReadString();

			Console.WriteLine($"{IServer.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient} with name {username}.");
			if (fromClient != clientIdCheck)
			{
				Console.WriteLine($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIdCheck})!");
			}

			FrontendServerSendsHandle.ConnectToOtherServer(fromClient, ServerPoolHandler.GetClosestServer(fromClient));
			Console.WriteLine($"Player \"{username}\" (ID: {fromClient}) is disconnecting from Frontend server");
			GC.SuppressFinalize(IServer.Clients[fromClient]);
			IServer.Clients[fromClient] = new FrontendClient(fromClient);
		}
	}
}
