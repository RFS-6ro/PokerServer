using UniCastCommonData.Network;

namespace RegionServer.ClientSide
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
