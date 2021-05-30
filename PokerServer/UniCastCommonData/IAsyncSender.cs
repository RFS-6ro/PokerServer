using UniCastCommonData.Handlers;

namespace UniCastCommonData
{
	public interface IAsyncSender<HANDLER, ENUM>
		where HANDLER : ISendMessageHandler<ENUM>, new()
	{
		ActorType ClientType { get; }

		HANDLER SendHandler { get; }
	}
}
