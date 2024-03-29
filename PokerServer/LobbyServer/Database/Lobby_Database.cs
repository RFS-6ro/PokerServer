﻿using System;
using System.Net;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace FrontendDistributionServer.Database
{
	public class Lobby_Database : AbstractTCPClient<
		ReceiveFromDatabaseHandler,
		SendToDatabaseHandler,
		Lobby_Database>
	{
		public override ActorType ServerType => ActorType.Databaase;

		public override ActorType ClientType => ActorType.LobbyServer;

		public Lobby_Database(IPAddress address, int port) : base(address, port)
		{
		}

		public Lobby_Database(string address, int port) : base(address, port)
		{
		}
	}
}
