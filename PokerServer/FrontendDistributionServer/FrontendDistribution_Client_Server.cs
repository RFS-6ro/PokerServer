using System;
using UniCastCommonData;
using UniCastCommonData.Network;

namespace FrontendDistributionServer
{
	public class FrontendDistribution_Client_Server : TcpServer, IAsyncReceiver<>, IAsyncSender<>
	{
	}
}
