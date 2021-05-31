using UniCastCommonData.Handlers;

namespace UniCastCommonData
{
	public interface IAsyncReceiver<HANDLER, ENUM>
		where HANDLER : IReceivedMessageHandler<ENUM>, new()
	{
		ActorType ServerType { get; }

		HANDLER ReceiveHandler { get; }
	}
}
