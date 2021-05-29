using System;
using System.Net;
using UniCastCommonData.Network;
using UniCastCommonData;

namespace RegionServer
{
	public class Region_Client_Server : TcpServer, IAsyncReceiver<ReceiveFromClientHandler>, IAsyncSender<SendToClientHandler>
	{
		public Region_Client_Server(IPAddress address, int port) : base(address, port)
		{
		}

		public Region_Client_Server(string address, int port) : base(address, port)
		{
		}
	}
}
