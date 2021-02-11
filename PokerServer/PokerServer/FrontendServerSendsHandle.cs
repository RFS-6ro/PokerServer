using System;
using Network;

namespace FrontendServer
{
	public static class FrontendServerSendsHandle
	{
		public enum FrontendServerSendingPacketsType
		{
			WelcomeToPlayer = 1,
			BestGameServerAddress = 2,
		}

		public static void Welcome(int playerId, string message)
		{
			using (Packet packet = new Packet((int)FrontendServerSendingPacketsType.WelcomeToPlayer))
			{
				packet.Write(playerId);
				packet.Write(message);

				FrontendServerSendHandlers.SendTCPData(playerId, packet);
			}
		}

		public static void ConnectToOtherServer(int playerId, ServerIdentifierData data)
		{
			using (Packet packet = new Packet((int)FrontendServerSendingPacketsType.BestGameServerAddress))
			{
				packet.Write(playerId);
				packet.Write(data);

				FrontendServerSendHandlers.SendTCPData(playerId, packet);
			}
		}
	}
}
