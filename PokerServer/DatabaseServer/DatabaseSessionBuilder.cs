using UniCastCommonData.Network;

namespace DatabaseServer
{
	public class DatabaseSessionBuilder : ITCPSessionBuilder
	{
		public DatabaseSessionBuilder() { }

		public TcpSession Create(TcpServer server)
		{
			return new Database_Session(server);
		}
	}
}