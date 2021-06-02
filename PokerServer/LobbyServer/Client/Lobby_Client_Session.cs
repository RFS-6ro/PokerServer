using UniCastCommonData.Network;

namespace LobbyServer.Client
{
	public class Lobby_Client_Session : TcpSession
	{
		public Lobby_Client_Session(TcpServer server) : base(server)
		{
		}
	}
}
