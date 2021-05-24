using System;
using NetCoreServer;

namespace LobbyServer
{
	public class Lobby_Session : TcpSession
	{
		public Lobby_Session(TcpServer server) : base(server)
		{
		}
	}
}
