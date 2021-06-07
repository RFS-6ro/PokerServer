using System;
using System.Net;
using UniCastCommonData.Handlers;
using UniCastCommonData.Packet.InitialDatas;

namespace UniCastCommonData.Network
{
	public abstract class AbstractTCPClient<RECEIVE_HANDLER, SEND_HANDLER, INSTANCE_TYPE>
		: TcpClient,
		  IStaticInstance<INSTANCE_TYPE>,
		  IAsyncReceiver<RECEIVE_HANDLER, int>,
		  IAsyncSender<SEND_HANDLER, int>

		where RECEIVE_HANDLER : IReceivedMessageHandler<int>, new()
		where SEND_HANDLER : ISendMessageHandler<int>, new()
		where INSTANCE_TYPE : AbstractTCPClient<RECEIVE_HANDLER, SEND_HANDLER, INSTANCE_TYPE>
	{

		public abstract ActorType ServerType { get; }
		public abstract ActorType ClientType { get; }

		RECEIVE_HANDLER IAsyncReceiver<RECEIVE_HANDLER, int>.ReceiveHandler { get; } = new RECEIVE_HANDLER();
		SEND_HANDLER IAsyncSender<SEND_HANDLER, int>.SendHandler { get; } = new SEND_HANDLER();

		private IReceivedMessageHandler<int> _receiveHandler;
		public IReceivedMessageHandler<int> ReceiveHandler => _receiveHandler;
		private ISendMessageHandler<int> _sendHandler;
		public ISendMessageHandler<int> SendHandler => _sendHandler;

		public AbstractTCPClient(IPAddress address, int port) : base(address, port) { InitReferences(); }
		public AbstractTCPClient(string address, int port) : base(address, port) { InitReferences(); }
		protected AbstractTCPClient(IPEndPoint endpoint) : base(endpoint) { InitReferences(); }

		protected virtual void InitReferences()
		{
			IStaticInstance<INSTANCE_TYPE>.Instance = (INSTANCE_TYPE)this;
			_sendHandler = ((IAsyncSender<SEND_HANDLER, int>)this).SendHandler;
			_receiveHandler = ((IAsyncReceiver<RECEIVE_HANDLER, int>)this).ReceiveHandler;
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
