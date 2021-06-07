using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient.Lobby.Handlers
{
	public enum lobbyTOclient
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
			Handlers.Add((int)lobbyTOclient.Test, Test);
			Handlers.Add((int)lobbyTOclient.Connect, Connect);
			Handlers.Add((int)lobbyTOclient.Disconnect, Disconnect);
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
				Guid senderGuid = new Guid(packet.Read(16));
				Guid receiverGuid = new Guid(packet.Read(16));
				IStaticInstance<Client_Lobby>.Instance.SetId(receiverGuid);

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