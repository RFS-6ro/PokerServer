using System;
using System.Net;
using NetCoreServer;
using UniCastCommonData;

namespace RegionServer
{
	public class Region_Client : TcpClient, IAsyncReceiver, IAsyncSender
	{
		public Region_Client(IPEndPoint endpoint) : base(endpoint)
		{
		}

		public Region_Client(IPAddress address, int port) : base(address, port)
		{
		}

		public Region_Client(string address, int port) : base(address, port)
		{
		}
	}
}
