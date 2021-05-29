using UniCastCommonData.Handlers;

namespace UniCastCommonData
{
	public interface IAsyncReceiver<HANDLER, ENUM>
		where HANDLER : IReceivedMessageHandler<ENUM>, new()
	{
		ActorType ReceiverType { get; }

		HANDLER ReceiveHandler { get; }
	}
}
