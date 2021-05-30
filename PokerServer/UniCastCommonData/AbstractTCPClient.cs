using System.Net;
using UniCastCommonData.Handlers;

namespace UniCastCommonData.Network
{
	public abstract class AbstractTCPClient<RECEIVE_HANDLER, RECEIVE_ENUM, SEND_HANDLER, SEND_ENUM>
		: TcpClient,
		  IAsyncReceiver<RECEIVE_HANDLER, RECEIVE_ENUM>,
		  IAsyncSender<SEND_HANDLER, SEND_ENUM>
		where RECEIVE_HANDLER : IReceivedMessageHandler<RECEIVE_ENUM>, new()
		where SEND_HANDLER : ISendMessageHandler<SEND_ENUM>, new()
	{
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

		protected virtual void InitReferences()
		{
			_sendHandler = ((IAsyncSender<SEND_HANDLER, SEND_ENUM>)this).SendHandler;
			_receiveHandler = ((IAsyncReceiver<RECEIVE_HANDLER, RECEIVE_ENUM>)this).ReceiveHandler;
		}
	}
}
