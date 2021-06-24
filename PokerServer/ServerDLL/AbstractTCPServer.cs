using System;
using System.Net;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace UniCastCommonData.Network
{
	public abstract class AbstractTCPServer<RECEIVE_HANDLER, SEND_HANDLER, INSTANCE_TYPE>
		: TcpServer,
		  IAsyncReceiver<RECEIVE_HANDLER>,
		  IAsyncSender<SEND_HANDLER>
		where RECEIVE_HANDLER : SessionReceiveHandlerBase<INSTANCE_TYPE>, new()
		where SEND_HANDLER : SessionSender<INSTANCE_TYPE>, new()
		where INSTANCE_TYPE : AbstractTCPServer<RECEIVE_HANDLER, SEND_HANDLER, INSTANCE_TYPE>
	{
		public abstract ActorType ServerType { get; }
		public abstract ActorType ClientType { get; }

		RECEIVE_HANDLER IAsyncReceiver<RECEIVE_HANDLER>.ReceiveHandler { get; } = new RECEIVE_HANDLER();
		SEND_HANDLER IAsyncSender<SEND_HANDLER>.SendHandler { get; } = new SEND_HANDLER();

		private SessionReceiveHandlerBase<INSTANCE_TYPE> _receiveHandler;
		public SessionReceiveHandlerBase<INSTANCE_TYPE> ReceiveHandler => _receiveHandler;
		private SessionSender<INSTANCE_TYPE> _sendHandler;
		public SessionSender<INSTANCE_TYPE> SendHandler => _sendHandler;

		public AbstractTCPServer(IPAddress address, int port) : base(address, port) { InitReferences(); }
		public AbstractTCPServer(string address, int port) : base(address, port) { InitReferences(); }
		protected AbstractTCPServer(IPEndPoint endpoint) : base(endpoint) { InitReferences(); }

		public static INSTANCE_TYPE Instance { get; protected set; }

		protected virtual void InitReferences()
		{
			Instance = (INSTANCE_TYPE)this;
			_sendHandler = ((IAsyncSender<SEND_HANDLER>)this).SendHandler;
			_sendHandler.Server = Instance;
			_receiveHandler = ((IAsyncReceiver<RECEIVE_HANDLER>)this).ReceiveHandler;
		}

		//Sessions.First(
		//	(x) =>
		//		(x.Value as FrontendDistribution_Session).Type == SenderType.Client)
		//	.Value.SendAsync(new byte[] { });

		protected override void OnConnected(TcpSession session)
		{
			SendHandler.Handlers[1]?.Invoke(
				new InitialSendingData(
					session.Id, Id,
					ServerType, 1
				));
		}
	}
}
