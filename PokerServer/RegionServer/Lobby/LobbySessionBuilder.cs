using System;
using UniCastCommonData.Network;

namespace RegionServer.LobbySide
{
	public class LobbySessionBuilder : ITCPSessionBuilder
	{
		public LobbySessionBuilder() { }

		public TcpSession Create(TcpServer server)
		{
			return new Region_Lobby_Session(server);
		}
	}
}
