using UniCastCommonData.Network;

namespace RegionServer.Lobby
{
	public class Region_Lobby_Session : TcpSession
	{
		public Region_Lobby_Session(TcpServer server) : base(server)
		{
		}
	}
}
