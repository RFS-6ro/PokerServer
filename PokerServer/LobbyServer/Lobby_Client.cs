using System;
using System.Net;
using NetCoreServer;
using UniCastCommonData;

namespace LobbyServer
{
	public class Lobby_Client : TcpClient, IAsyncReceiver, IAsyncSender
	{
		public Lobby_Client(IPEndPoint endpoint) : base(endpoint)
		{
		}

		public Lobby_Client(IPAddress address, int port) : base(address, port)
		{
		}

		public Lobby_Client(string address, int port) : base(address, port)
		{
		}

		public SenderType Type => throw new NotImplementedException();
	}
}
