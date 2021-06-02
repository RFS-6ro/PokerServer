﻿using System;
using System.Net;
using RegionServer.Lobby.Handlers;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace RegionServer.Lobby
{
	public class Region_Lobby_Server : AbstractTCPServer<
		LobbySessionBuilder,
		ReceiveFromLobbyHandler, int,
		SendToLobbyHandler, int,
		Region_Lobby_Server>
	{
		public override ActorType ServerType => ActorType.RegionServer;

		public override ActorType ClientType => ActorType.LobbyServer;

		public Region_Lobby_Server(IPAddress address, int port) : base(address, port)
		{
		}
	}
}