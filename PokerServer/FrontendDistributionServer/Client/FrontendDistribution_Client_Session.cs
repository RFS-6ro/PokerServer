using System;
using FrontendDistributionServer.Client.Handlers;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace FrontendDistributionServer.Client
{
	public class FrontendDistribution_Client_Session : TcpSession
	{
		public FrontendDistribution_Client_Session(TcpServer server) : base(server)
		{
		}
	}
}
