using UniCastCommonData;
using UniCastCommonData.Network.MessageHandlers;

namespace RegionServer.Lobby.Handlers
{
	public enum regionTOlobby
	{
		None = 0,

		Count,


		Test
	}

	public class SendToLobbyHandler : SessionSender<Region_Lobby_Server>
	{
		public SendToLobbyHandler()
		{
			Handlers.Add((int)regionTOlobby.Test, Test);
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