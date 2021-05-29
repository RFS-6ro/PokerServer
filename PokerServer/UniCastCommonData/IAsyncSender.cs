using UniCastCommonData.Handlers;

namespace UniCastCommonData
{
	public interface IAsyncSender<HANDLER, ENUM>
		where HANDLER : ISendMessageHandler<ENUM>, new()
	{
		ActorType SenderType { get; }

		HANDLER SendHandler { get; }
	}
}
