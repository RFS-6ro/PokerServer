using System;
using System.Net;
using NetCoreServer;
using UniCastCommonData;

namespace TestingClient
{
	public class Client_FrontendDistributor : TcpClient, IAsyncReceiver, IAsyncSender
	{
		public Client_FrontendDistributor(IPEndPoint endpoint) : base(endpoint)
		{
		}

		public Client_FrontendDistributor(IPAddress address, int port) : base(address, port)
		{
		}

		public Client_FrontendDistributor(string address, int port) : base(address, port)
		{
		}

		public SenderType Type => throw new NotImplementedException();
	}
}
