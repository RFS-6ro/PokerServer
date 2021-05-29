using System;
using System.Net;
using UniCastCommonData.Network;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using LobbyServer.Handlers;

namespace LobbyServer
{
	public class Lobby_Region : AbstractTCPClient<ReceiveFromRegionHandler, SendToRegionHandler>
	{
		public override ActorType SenderType => ActorType.RegionServer;

		public override ActorType ReceiverType => ActorType.LobbyServer;

		public Lobby_Region(IPAddress address, int port) : base(address, port)
		{
		}

		public Lobby_Region(string address, int port) : base(address, port)
		{
		}
	}
}
