using UniCastCommonData;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace TestingClient.Lobby.Handlers
{
	public enum clientTOlobby
	{
		None = 0,

		Connect,
		SendTurn,
		Disconnect,

		Count,


		Test
	}

	public class SendToLobbyHandler : ClientSender<Client_Lobby>
	{
		public SendToLobbyHandler()
		{
			Handlers.Add((int)clientTOlobby.Test, Test);
			Handlers.Add((int)clientTOlobby.Connect, Connect);
			Handlers.Add((int)clientTOlobby.SendTurn, SendTurn);
			Handlers.Add((int)clientTOlobby.Disconnect, Disconnect);
		}

		private void Connect(InitialSendingData data)
		{
			SendAsync(data, new byte[][]
			{
				Client_Lobby.Instance.Name.ToByteArray()
			});
		}

		private void SendTurn(InitialSendingData data)
		{
			SendAsync(data, new byte[][]
			{
				InputModel.GetTurn()
			});
		}

		private void Disconnect(InitialSendingData data)
		{
			SendAsync(data, new byte[][]
			{
				Client_Lobby.Instance.Name.ToByteArray()
			});
		}

		private void Test(InitialSendingData data)
		{
			SendAsync(data, new byte[][]
			{
				GetType().ToString().ToByteArray()
			});
		}
	}
}