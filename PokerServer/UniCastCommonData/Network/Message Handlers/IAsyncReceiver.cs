using UniCastCommonData.Handlers;

namespace UniCastCommonData
{
	public interface IAsyncReceiver<HANDLER>
		where HANDLER : IReceivedMessageHandler, new()
	{
		ActorType ServerType { get; }

		HANDLER ReceiveHandler { get; }
	}
}
