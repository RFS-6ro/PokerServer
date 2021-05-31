using System.Net;
using UniCastCommonData.Handlers;

namespace UniCastCommonData.Network
{
	public abstract class AbstractTCPClient<RECEIVE_HANDLER, RECEIVE_ENUM, SEND_HANDLER, SEND_ENUM, INSTANCE_TYPE>
		: TcpClient,
		  IAsyncReceiver<RECEIVE_HANDLER, RECEIVE_ENUM>,
		  IAsyncSender<SEND_HANDLER, SEND_ENUM>

		where RECEIVE_HANDLER : IReceivedMessageHandler<RECEIVE_ENUM>, new()
		where SEND_HANDLER : ISendMessageHandler<SEND_ENUM>, new()
		where INSTANCE_TYPE : AbstractTCPClient<RECEIVE_HANDLER, RECEIVE_ENUM, SEND_HANDLER, SEND_ENUM, INSTANCE_TYPE>
	{
		public static INSTANCE_TYPE Instance { get; protected set; }

		public abstract ActorType ServerType { get; }
		public abstract ActorType ClientType { get; }

		RECEIVE_HANDLER IAsyncReceiver<RECEIVE_HANDLER, RECEIVE_ENUM>.ReceiveHandler { get; } = new RECEIVE_HANDLER();
		SEND_HANDLER IAsyncSender<SEND_HANDLER, SEND_ENUM>.SendHandler { get; } = new SEND_HANDLER();

		private IReceivedMessageHandler<RECEIVE_ENUM> _receiveHandler;
		public IReceivedMessageHandler<RECEIVE_ENUM> ReceiveHandler => _receiveHandler;
		private ISendMessageHandler<SEND_ENUM> _sendHandler;
		public ISendMessageHandler<SEND_ENUM> SendHandler => _sendHandler;

		public AbstractTCPClient(IPAddress address, int port) : base(address, port) { InitReferences(); }
		public AbstractTCPClient(string address, int port) : base(address, port) { InitReferences(); }
		protected AbstractTCPClient(IPEndPoint endpoint) : base(endpoint) { InitReferences(); }

		protected virtual void InitReferences()
		{
			Instance = (INSTANCE_TYPE)this;
			_sendHandler = ((IAsyncSender<SEND_HANDLER, SEND_ENUM>)this).SendHandler;
			_receiveHandler = ((IAsyncReceiver<RECEIVE_HANDLER, RECEIVE_ENUM>)this).ReceiveHandler;
		}
	}
}
