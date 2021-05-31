using UniCastCommonData.Network;

namespace LobbyServer.ClientSide
{
	public class Lobby_Client_Session : TcpSession
	{
		public Lobby_Client_Session(TcpServer server) : base(server)
		{
		}
	}
}
