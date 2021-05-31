using UniCastCommonData.Network;

namespace RegionServer.ClientSide
{
	public class Region_Client_Session : TcpSession
	{
		public Region_Client_Session(TcpServer server) : base(server)
		{
		}
	}
}
