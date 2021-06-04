using System;
using System.Net;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace FrontendDistributionServer.Database
{
	public class LobbyDatabaseClient : AbstractTCPClient<
		ReceiveFromDatabaseHandler, int,
		SendToDatabaseHandler, int,
		LobbyDatabaseClient>
	{
		public override ActorType ServerType => ActorType.Databaase;

		public override ActorType ClientType => ActorType.LobbyServer;

		public LobbyDatabaseClient(IPAddress address, int port) : base(address, port)
		{
		}

		public LobbyDatabaseClient(string address, int port) : base(address, port)
		{
		}
	}
}
