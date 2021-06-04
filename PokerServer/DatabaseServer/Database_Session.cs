using System;
using UniCastCommonData.Network;

namespace DatabaseServer
{
	public class Database_Session : TcpSession
	{
		public Database_Session(TcpServer server) : base(server)
		{
		}
	}
}
