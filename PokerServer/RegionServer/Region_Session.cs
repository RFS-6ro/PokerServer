using System;
using NetCoreServer;

namespace RegionServer
{
	public class Region_Session : TcpSession
	{
		public Region_Session(TcpServer server) : base(server)
		{
		}
	}
}
