using System;
using System.Net;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace UniCastCommonData.Network
{
	public abstract class AbstractTCPClient<RECEIVE_HANDLER, SEND_HANDLER, INSTANCE_TYPE>
		: TcpClient,
		  IAsyncReceiver<RECEIVE_HANDLER>,
		  IAsyncSender<SEND_HANDLER>

		where RECEIVE_HANDLER : ClientReceiveHandlerBase<INSTANCE_TYPE>, new()
		where SEND_HANDLER : ClientSender<INSTANCE_TYPE>, new()
		where INSTANCE_TYPE : AbstractTCPClient<RECEIVE_HANDLER, SEND_HANDLER, INSTANCE_TYPE>
	{
		public Guid ServerId { get; set; }

		public abstract ActorType ServerType { get; }
		public abstract ActorType ClientType { get; }

		RECEIVE_HANDLER IAsyncReceiver<RECEIVE_HANDLER>.ReceiveHandler { get; } = new RECEIVE_HANDLER();
		SEND_HANDLER IAsyncSender<SEND_HANDLER>.SendHandler { get; } = new SEND_HANDLER();

		private ClientReceiveHandlerBase<INSTANCE_TYPE> _receiveHandler;
		public ClientReceiveHandlerBase<INSTANCE_TYPE> ReceiveHandler => _receiveHandler;
		private ClientSender<INSTANCE_TYPE> _sendHandler;
		public ClientSender<INSTANCE_TYPE> SendHandler => _sendHandler;

		public AbstractTCPClient(IPAddress address, int port) : base(address, port) { InitReferences(); }
		public AbstractTCPClient(string address, int port) : base(address, port) { InitReferences(); }
		protected AbstractTCPClient(IPEndPoint endpoint) : base(endpoint) { InitReferences(); }

		protected virtual void InitReferences()
		{
			_sendHandler = ((IAsyncSender<SEND_HANDLER>)this).SendHandler;
			_receiveHandler = ((IAsyncReceiver<RECEIVE_HANDLER>)this).ReceiveHandler;
		}

		protected override void OnConnected()
		{
			SendHandler.Handlers[1]?.Invoke(
				new InitialSendingData(
					Guid.Empty, Id,
					ClientType, 1
				));
		}
	}
}
