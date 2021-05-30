using UniCastCommonData.Network;

namespace RegionServer
{
	public class ClientSessionBuilder : ITCPSessionBuilder
	{
		public ClientSessionBuilder() { }

		public TcpSession Create(TcpServer server)
		{
			return new Region_Client_Session(server);
		}
	}
}
