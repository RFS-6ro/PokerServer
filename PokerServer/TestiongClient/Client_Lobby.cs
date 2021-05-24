using System;
using System.Net;
using NetCoreServer;
using UniCastCommonData;

namespace TestiongClient
{
	public class Client_Lobby : TcpClient, IAsyncReceiver, IAsyncSender
	{
		public Client_Lobby(IPEndPoint endpoint) : base(endpoint)
		{
		}

		public Client_Lobby(IPAddress address, int port) : base(address, port)
		{
		}

		public Client_Lobby(string address, int port) : base(address, port)
		{
		}
	}
}
