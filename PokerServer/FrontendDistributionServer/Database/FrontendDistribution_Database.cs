using System;
using System.Net;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace FrontendDistributionServer.Database
{
	public class FrontendDistribution_Database : AbstractTCPClient<
		ReceiveFromDatabaseHandler, int,
		SendToDatabaseHandler, int,
		FrontendDistribution_Database>
	{
		public override ActorType ServerType => ActorType.Databaase;

		public override ActorType ClientType => ActorType.FrontendDistributionServer;

		public FrontendDistribution_Database(IPAddress address, int port) : base(address, port)
		{
		}

		public FrontendDistribution_Database(string address, int port) : base(address, port)
		{
		}
	}
}
