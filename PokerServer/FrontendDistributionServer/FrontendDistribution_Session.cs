using System;
using NetCoreServer;

namespace FrontendDistributionServer
{
	public class FrontendDistribution_Session : TcpSession
	{
		public FrontendDistribution_Session(TcpServer server) : base(server)
		{
		}
	}
}
