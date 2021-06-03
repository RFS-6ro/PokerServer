using System;
using System.Collections.Generic;
using UniCastCommonData.Network.MessageHandlers;

namespace UniCastCommonData.Handlers
{
	public interface ISendMessageHandler<T>
	{
		ISender Sender { get; set; }

		Dictionary<T, Action<InitialSendingData>> Handlers { get; }
	}
}
