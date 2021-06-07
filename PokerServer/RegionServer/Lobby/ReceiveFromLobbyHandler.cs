using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

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

	public class ReceiveFromLobbyHandler : IReceivedMessageHandler<int>
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; } = new Dictionary<int, Action<UniCastPacket>>();

		public ReceiveFromLobbyHandler()
		{
			Handlers.Add((int)lobbyTOregion.Test, Test);
			Handlers.Add((int)lobbyTOregion.Connect, Connect);
			Handlers.Add((int)lobbyTOregion.Disconnect, Disconnect);
		}

		private void Disconnect(UniCastPacket packet)
		{
			ThreadManager.ExecuteOnMainThread(() =>
			{
			});
		}

		private void Connect(UniCastPacket packet)
		{
			ThreadManager.ExecuteOnMainThread(() =>
			{
			});
		}

		private void Test(UniCastPacket packet)
		{
			ThreadManager.ExecuteOnMainThread(() =>
			{
				Guid guid = new Guid(packet.Read(16));

				string message = packet.ReadString();

				Console.WriteLine(guid + "|" + message);
			});
		}
	}
}