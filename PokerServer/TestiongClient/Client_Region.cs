using System;
using System.Net;
using NetCoreServer;
using UniCastCommonData;

namespace TestiongClient
{
	public class Client_Region : TcpClient, IAsyncReceiver, IAsyncSender
	{
		public Client_Region(IPEndPoint endpoint) : base(endpoint)
		{
		}

		public Client_Region(IPAddress address, int port) : base(address, port)
		{
		}

		public Client_Region(string address, int port) : base(address, port)
		{
		}
	}
}
