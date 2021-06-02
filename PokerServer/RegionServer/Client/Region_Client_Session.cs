using UniCastCommonData.Network;

namespace RegionServer.Client
{
	public class Region_Client_Session : TcpSession
	{
		public Region_Client_Session(TcpServer server) : base(server)
		{
		}
	}
}
