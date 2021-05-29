using System;
using System.Linq;
using System.Net;
using UniCastCommonData.Network;
using UniCastCommonData;

namespace FrontendDistributionServer
{
	public class FrontendDistribution_Server : TcpServer
	{

		public FrontendDistribution_Server(IPAddress address, int port) : base(address, port)
		{
		}
		private static bool _isRunning = false;

		public static bool IsRunning => _isRunning;

		public override bool Start()
		{
			_isRunning = base.Start();
			//Sessions.First(
			//	(x) =>
			//		(x.Value as FrontendDistribution_Session).Type == SenderType.Client)
			//	.Value.SendAsync(new byte[] { });
			return _isRunning;
		}

		protected override TcpSession CreateSession()
		{
			return new FrontendDistribution_Session(this);
		}

		public override bool Stop()
		{
			_isRunning = base.Stop();
			return _isRunning;
		}
	}
}
