using System;
using System.Net;
using FrontendDistributionServer.Handlers;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace FrontendDistributionServer
{
	public class FrontendDistribution_Region_Server : TcpServer, IAsyncReceiver<ReceiveFromRegionHandler>, IAsyncSender<SendToRegionHandler>
	{
		private static bool _isRunning = false;

		public static bool IsRunning => _isRunning;

		public ActorType ReceiverType => throw new NotImplementedException();

		public ReceiveFromRegionHandler ReceiveHandler => throw new NotImplementedException();

		public ActorType SenderType => throw new NotImplementedException();

		public SendToRegionHandler SendHandler => throw new NotImplementedException();

		public FrontendDistribution_Region_Server(IPAddress address, int port) : base(address, port)
		{
		}

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
