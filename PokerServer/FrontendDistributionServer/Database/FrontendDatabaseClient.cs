using System;
using System.Net;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace FrontendDistributionServer.Database
{
	public class FrontendDatabaseClient : AbstractTCPClient<
		ReceiveFromDatabaseHandler, int,
		SendToDatabaseHandler, int,
		FrontendDatabaseClient>
	{
		public override ActorType ServerType => ActorType.Databaase;

		public override ActorType ClientType => ActorType.FrontendDistributionServer;

		public FrontendDatabaseClient(IPAddress address, int port) : base(address, port)
		{
		}

		public FrontendDatabaseClient(string address, int port) : base(address, port)
		{
		}
	}
}
