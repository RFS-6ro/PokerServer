using System;
using System.Net;
using NetCoreServer;
using UniCastCommonData;

namespace LobbyServer
{
	public class Lobby_Server : TcpServer, IAsyncReceiver, IAsyncSender
	{
		public Lobby_Server(IPEndPoint endpoint) : base(endpoint)
		{
		}

		public Lobby_Server(IPAddress address, int port) : base(address, port)
		{
		}

		public Lobby_Server(string address, int port) : base(address, port)
		{
		}

		public SenderType Type => throw new NotImplementedException();
	}
}
