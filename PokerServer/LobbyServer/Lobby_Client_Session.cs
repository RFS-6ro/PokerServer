using System;
using UniCastCommonData.Network;

namespace LobbyServer
{
	public class Lobby_Client_Session : TcpSession
	{
		public Lobby_Client_Session(TcpServer server) : base(server)
		{
		}
	}
}
