using System;
using UniCastCommonData;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace RegionServer.Lobby.Handlers
{
	public enum regionTOlobby
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class SendToLobbyHandler : SessionSender<Region_Lobby_Server>
	{
		public SendToLobbyHandler()
		{
			Handlers.Add((int)regionTOlobby.Test, Test);
			Handlers.Add((int)regionTOlobby.Connect, Connect);
			Handlers.Add((int)regionTOlobby.Disconnect, Disconnect);
		}

		private void Disconnect(InitialSendingData data)
		{
			using (UniCastPacket packet = new UniCastPacket(data))
			{


				Sender.SendAsync(packet);
			}
		}

		private void Connect(InitialSendingData data)
		{
			using (UniCastPacket packet = new UniCastPacket(data))
			{


				Sender.SendAsync(packet);
			}
		}

		private void Test(InitialSendingData data)
		{
			using (UniCastPacket packet = new UniCastPacket(data))
			{
				packet.Write(GetType().ToString());

				Sender.SendAsync(packet);
			}
		}
	}
}