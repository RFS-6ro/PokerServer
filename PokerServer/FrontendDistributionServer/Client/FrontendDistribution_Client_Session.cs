using System;
using FrontendDistributionServer.ClientSide.Handlers;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace FrontendDistributionServer.ClientSide
{
	public class FrontendDistribution_Client_Session : TcpSession
	{
		public FrontendDistribution_Client_Session(TcpServer server) : base(server)
		{
		}
	}
}
