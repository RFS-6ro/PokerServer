using System;
using System.Collections.Generic;

namespace UniCastCommonData.Handlers
{
	public interface IHandlersKeeper
	{
		Dictionary<SenderType, IReceivedMessageHandler> TypedReceiveHandlers { get; }

		Dictionary<SenderType, ISendMessageHandler> TypedSendHandlers { get; }
	}
}
