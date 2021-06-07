using System;
using System.Net;
using UniCastCommonData.Handlers;
using UniCastCommonData.Packet.InitialDatas;

namespace UniCastCommonData.Network
{
	public abstract class AbstractTCPServer<SESSION_BUILDER, RECEIVE_HANDLER, SEND_HANDLER, INSTANCE_TYPE>
		: TcpServer,
		  IStaticInstance<INSTANCE_TYPE>,
		  IAsyncReceiver<RECEIVE_HANDLER, int>,
		  IAsyncSender<SEND_HANDLER, int>
		where SESSION_BUILDER : ITCPSessionBuilder, new()
		where RECEIVE_HANDLER : IReceivedMessageHandler<int>, new()
		where SEND_HANDLER : ISendMessageHandler<int>, new()
		where INSTANCE_TYPE : AbstractTCPServer<SESSION_BUILDER, RECEIVE_HANDLER, SEND_HANDLER, INSTANCE_TYPE>
	{
		protected ITCPSessionBuilder _builder { get; set; }

		public abstract ActorType ServerType { get; }
		public abstract ActorType ClientType { get; }

		RECEIVE_HANDLER IAsyncReceiver<RECEIVE_HANDLER, int>.ReceiveHandler { get; } = new RECEIVE_HANDLER();
		SEND_HANDLER IAsyncSender<SEND_HANDLER, int>.SendHandler { get; } = new SEND_HANDLER();

		private IReceivedMessageHandler<int> _receiveHandler;
		public IReceivedMessageHandler<int> ReceiveHandler => _receiveHandler;
		private ISendMessageHandler<int> _sendHandler;
		public ISendMessageHandler<int> SendHandler => _sendHandler;

		public AbstractTCPServer(IPAddress address, int port) : base(address, port) { InitReferences(); }
		public AbstractTCPServer(string address, int port) : base(address, port) { InitReferences(); }
		protected AbstractTCPServer(IPEndPoint endpoint) : base(endpoint) { InitReferences(); }

		protected virtual void InitReferences()
		{
			IStaticInstance<INSTANCE_TYPE>.Instance = (INSTANCE_TYPE)this;
			_sendHandler = ((IAsyncSender<SEND_HANDLER, int>)this).SendHandler;
			_receiveHandler = ((IAsyncReceiver<RECEIVE_HANDLER, int>)this).ReceiveHandler;
		}

		//Sessions.First(
		//	(x) =>
		//		(x.Value as FrontendDistribution_Session).Type == SenderType.Client)
		//	.Value.SendAsync(new byte[] { });

		protected virtual void ConfigBuilder(ITCPSessionBuilder builder)
		{
			_builder = builder;
		}

		protected override TcpSession CreateSession()
		{
			if (_builder == null)
			{
				_builder = new SESSION_BUILDER();
			}

			return _builder.Create(this);
		}

		protected override void OnConnected(TcpSession session)
		{
			SendHandler.Handlers[1]?.Invoke(
				new InitialSendingData(
					session.Id, Id,
					ClientType, 1
				));
		}
	}
}
