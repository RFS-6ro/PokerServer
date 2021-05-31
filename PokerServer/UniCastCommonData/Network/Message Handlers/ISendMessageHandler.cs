using System;
using System.Collections.Generic;

namespace UniCastCommonData.Handlers
{
	public interface ISendMessageHandler<T>
	{
		Dictionary<T, Action<InitialSendingData>> Handlers { get; }
	}
}
