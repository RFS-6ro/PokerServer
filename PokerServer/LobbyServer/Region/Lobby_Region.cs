using System.Net;
using UniCastCommonData.Network;
using UniCastCommonData.Handlers;
using LobbyServer.RegionSide.Handlers;

namespace LobbyServer.RegionSide
{
	public class Lobby_Region : AbstractTCPClient<
		ReceiveFromRegionHandler, int,
		SendToRegionHandler, int,
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
