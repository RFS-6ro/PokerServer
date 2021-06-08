using System;
using System.Collections.Generic;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace UniCastCommonData.Handlers
{
	public interface ISendMessageHandler<T>
	{
		ISender Sender { get; }

		Dictionary<T, Action<InitialSendingData>> Handlers { get; }
	}
}
