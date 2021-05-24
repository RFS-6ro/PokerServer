using System;
using System.Net;
using NetCoreServer;
using UniCastCommonData;

namespace FrontendDistributionServer
{
	public class FrontendDistribution_Server : TcpServer, IAsyncReceiver, IAsyncSender
	{
		public FrontendDistribution_Server(IPEndPoint endpoint) : base(endpoint)
		{
		}

		public FrontendDistribution_Server(IPAddress address, int port) : base(address, port)
		{
		}

		public FrontendDistribution_Server(string address, int port) : base(address, port)
		{
		}
	}
}
