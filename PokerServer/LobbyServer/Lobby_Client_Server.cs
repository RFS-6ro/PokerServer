using System;
using System.Net;
using UniCastCommonData.Network;
using UniCastCommonData;

namespace LobbyServer
{
	public class Lobby_Client_Server : TcpServer, IAsyncReceiver<>, IAsyncSender<>
	{
		public Lobby_Client_Server(IPAddress address, int port) : base(address, port)
		{
		}

		public Lobby_Client_Server(string address, int port) : base(address, port)
		{
		}
	}
}
