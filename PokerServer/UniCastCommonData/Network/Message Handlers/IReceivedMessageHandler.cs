using System;
using System.Collections.Generic;

namespace UniCastCommonData.Handlers
{
	public interface IReceivedMessageHandler<T>
	{
		Dictionary<T, Action<UniCastPacket>> Handlers { get; }

		void Receive(int action, UniCastPacket packet);
	}
}
