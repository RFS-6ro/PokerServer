using System;
using System.Net;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace DatabaseServer
{
	public class Database_Server : AbstractTCPServer<
		DatabaseSessionBuilder,
		ReceiveHandler,
		SendHandler,
		Database_Server>
	{
		public override ActorType ServerType => ActorType.Databaase;

		public override ActorType ClientType => ActorType.Any;

		public Database_Server(IPAddress address, int port) : base(address, port)
		{
		}
	}
}