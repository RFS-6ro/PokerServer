using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;

namespace RegionServer.Lobby.Handlers
{
	public enum lobbyTOregion
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class ReceiveFromLobbyHandler : SessionReceiveHandlerBase<Region_Lobby_Server>
	{
		public ReceiveFromLobbyHandler()
		{
			Handlers.Add((int)lobbyTOregion.Test, Test);
			Handlers.Add((int)lobbyTOregion.Connect, Connect);
			Handlers.Add((int)lobbyTOregion.Disconnect, Disconnect);
		}

		private void Disconnect(UniCastPacket packet)
		{
		}

		private void Connect(UniCastPacket packet)
		{
		}

		private void Test(UniCastPacket packet)
		{
			Guid receiverGuid = new Guid(packet.Read(16));

			string message = packet.ReadString();

			Console.WriteLine(receiverGuid + "|" + message);
		}
	}
}