using System;
using UniCastCommonData.Network;

namespace FrontendDistributionServer.Region
{
	public class RegionSessionBuilder : ITCPSessionBuilder
	{
		public RegionSessionBuilder() { }

		public TcpSession Create(TcpServer server)
		{
			return new FrontendDistribution_Region_Session(server);
		}
	}
}
