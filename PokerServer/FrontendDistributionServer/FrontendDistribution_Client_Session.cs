using System;
using UniCastCommonData.Network;

namespace FrontendDistributionServer
{
	public class FrontendDistribution_Client_Session : TcpSession
	{
		public FrontendDistribution_Client_Session(TcpServer server) : base(server)
		{
		}
	}
}
