using System;
using UniCastCommonData.Network;

namespace RegionServer
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
