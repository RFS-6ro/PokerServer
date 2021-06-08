using UniCastCommonData.Network;

namespace LobbyServer.Client
{
	public class Lobby_Client_Session : TcpSession
	{
		public string Name { get; }

		public Lobby_Client_Session(TcpServer server) : base(server) { }

		protected override void OnConnected()
		{
			PokerInitializator.AddNewPlayer()
		}
	}
}
