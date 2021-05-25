using System;
using System.Net;
using NetCoreServer;
using UniCastCommonData;

namespace RegionServer
{
	public class Region_Server : TcpServer, IAsyncReceiver, IAsyncSender
	{
		public Region_Server(IPEndPoint endpoint) : base(endpoint)
		{
		}

		public Region_Server(IPAddress address, int port) : base(address, port)
		{
		}

		public Region_Server(string address, int port) : base(address, port)
		{
		}

		public SenderType Type => throw new NotImplementedException();
	}
}
