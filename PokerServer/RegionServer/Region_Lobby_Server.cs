using System;
using UniCastCommonData;
using UniCastCommonData.Network;

namespace RegionServer
{
	public class Region_Lobby_Server : TcpServer, IAsyncReceiver<>, IAsyncSender<>
	{
	}
}
