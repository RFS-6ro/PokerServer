using System;
using System.Net;
using UniCastCommonData.Handlers;

namespace UniCastCommonData.Network
{
	public abstract class AbstractTCPServer<SESSION_BUILDER, RECEIVE_HANDLER, RECEIVE_ENUM, SEND_HANDLER, SEND_ENUM, INSTANCE_TYPE>
		: TcpServer,
		  IStaticInstance<INSTANCE_TYPE>,
		  IAsyncReceiver<RECEIVE_HANDLER, RECEIVE_ENUM>,
		  IAsyncSender<SEND_HANDLER, SEND_ENUM>
		where SESSION_BUILDER : ITCPSessionBuilder, new()
		where RECEIVE_HANDLER : IReceivedMessageHandler<RECEIVE_ENUM>, new()
		where SEND_HANDLER : ISendMessageHandler<SEND_ENUM>, new()
		where INSTANCE_TYPE : AbstractTCPServer<SESSION_BUILDER, RECEIVE_HANDLER, RECEIVE_ENUM, SEND_HANDLER, SEND_ENUM, INSTANCE_TYPE>
	{
		protected ITCPSessionBuilder _builder { get; set; }

		public abstract ActorType ServerType { get; }
		public abstract ActorType ClientType { get; }

		RECEIVE_HANDLER IAsyncReceiver<RECEIVE_HANDLER, RECEIVE_ENUM>.ReceiveHandler { get; } = new RECEIVE_HANDLER();
		SEND_HANDLER IAsyncSender<SEND_HANDLER, SEND_ENUM>.SendHandler { get; } = new SEND_HANDLER();

		private IReceivedMessageHandler<RECEIVE_ENUM> _receiveHandler;
		public IReceivedMessageHandler<RECEIVE_ENUM> ReceiveHandler => _receiveHandler;
		private ISendMessageHandler<SEND_ENUM> _sendHandler;
		public ISendMessageHandler<SEND_ENUM> SendHandler => _sendHandler;

		public AbstractTCPServer(IPAddress address, int port) : base(address, port) { InitReferences(); }
		public AbstractTCPServer(string address, int port) : base(address, port) { InitReferences(); }
		protected AbstractTCPServer(IPEndPoint endpoint) : base(endpoint) { InitReferences(); }

		protected virtual void InitReferences()
		{
			IStaticInstance<INSTANCE_TYPE>.Instance = (INSTANCE_TYPE)this;
			_sendHandler = ((IAsyncSender<SEND_HANDLER, SEND_ENUM>)this).SendHandler;
			_receiveHandler = ((IAsyncReceiver<RECEIVE_HANDLER, RECEIVE_ENUM>)this).ReceiveHandler;
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
	}
}
