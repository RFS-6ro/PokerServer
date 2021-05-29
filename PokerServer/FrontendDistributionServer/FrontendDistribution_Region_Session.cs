using System;
using UniCastCommonData.Network;

namespace FrontendDistributionServer
{
	public class FrontendDistribution_Region_Session : TcpSession
	{
		public FrontendDistribution_Region_Session(TcpServer server) : base(server)
		{
		}
	}
}
