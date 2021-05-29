using System;
using System.Net;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace UniCastCommonData
{
	public abstract class AbstractTCPServer<SESSION, RECEIVE_HANDLER, RECEIVE_ENUM, SEND_HANDLER, SEND_ENUM>
		: TcpServer,
		  IAsyncReceiver<RECEIVE_HANDLER, RECEIVE_ENUM>,
		  IAsyncSender<SEND_HANDLER, SEND_ENUM>
		where SESSION : TcpSession
		where RECEIVE_HANDLER : IReceivedMessageHandler<RECEIVE_ENUM>, new()
		where SEND_HANDLER : ISendMessageHandler<SEND_ENUM>, new()
	{
		protected static bool _isRunning = false;
		public static bool IsRunning => _isRunning;

		public abstract ActorType ReceiverType { get; }
		public abstract ActorType SenderType { get; }

		RECEIVE_HANDLER IAsyncReceiver<RECEIVE_HANDLER, RECEIVE_ENUM>.ReceiveHandler { get; } = new RECEIVE_HANDLER();
		SEND_HANDLER IAsyncSender<SEND_HANDLER, SEND_ENUM>.SendHandler { get; } = new SEND_HANDLER();

		private IReceivedMessageHandler<RECEIVE_ENUM> _receiveHandler;
		public IReceivedMessageHandler<RECEIVE_ENUM> ReceiveHandler => _receiveHandler;
		private ISendMessageHandler<SEND_ENUM> _sendHandler;
		public ISendMessageHandler<SEND_ENUM> SendHandler => _sendHandler;

		public AbstractTCPServer(IPAddress address, int port) : base(address, port) { InitReferences(); }
		public AbstractTCPServer(string address, int port) : base(address, port) { InitReferences(); }

		protected virtual void InitReferences()
		{
			_sendHandler = ((IAsyncSender<SEND_HANDLER, SEND_ENUM>)this).SendHandler;
			_receiveHandler = ((IAsyncReceiver<RECEIVE_HANDLER, RECEIVE_ENUM>)this).ReceiveHandler;
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
