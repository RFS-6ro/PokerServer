using System;
using System.Net;
using UniCastCommonData.Network;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using LobbyServer.Handlers;

namespace LobbyServer
{
	public class Lobby_Region : AbstractTCPClient<
		ReceiveFromRegionHandler,
		regionTOlobby,
		SendToRegionHandler,
		lobbyTOregion,
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
