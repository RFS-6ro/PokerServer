using System;
using System.Net;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace RegionServer
{
	public class Region_Client_Session : TcpSession
	{
		public Region_Client_Session(TcpServer server) : base(server)
		{
		}
	}
}
