using System;
using UniCastCommonData.Network;

namespace RegionServer
{
	public class Region_Lobby_Session : TcpSession
	{
		public Region_Lobby_Session(TcpServer server) : base(server)
		{
		}
	}
}
