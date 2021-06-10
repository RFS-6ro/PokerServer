using UniCastCommonData.Handlers;

namespace UniCastCommonData
{
	public interface IAsyncSender<HANDLER>
		where HANDLER : ISendMessageHandler, new()
	{
		ActorType ClientType { get; }

		HANDLER SendHandler { get; }
	}
}
