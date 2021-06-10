using System;
using System.Collections.Generic;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace UniCastCommonData.Handlers
{
	public interface ISendMessageHandler
	{
		ISender Sender { get; }

		Dictionary<int, Action<InitialSendingData>> Handlers { get; }

		void SendAsync(InitialSendingData data, byte[][] content);
	}
}
