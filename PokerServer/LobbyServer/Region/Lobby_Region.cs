﻿using System.Net;
using UniCastCommonData.Network;
using UniCastCommonData.Handlers;
using LobbyServer.Region.Handlers;

namespace LobbyServer.Region
{
	public class Lobby_Region : AbstractTCPClient<
		ReceiveFromRegionHandler,
		SendToRegionHandler,
		Lobby_Region>
	{
		public override ActorType ServerType => ActorType.RegionServer;

		public override ActorType ClientType => ActorType.LobbyServer;

		public Lobby_Region(IPAddress address, int port) : base(address, port)
		{
		}

		public Lobby_Region(string address, int port) : base(address, port)
		{
		}
	}
}
