using System;
using System.Collections.Generic;

namespace UniCastCommonData.Handlers
{
	public interface IReceivedMessageHandler<T>
	{
		Dictionary<T, Action<UniCastPacket>> Handlers { get; }

		void Receive(T action, UniCastPacket packet, Guid guid);
	}
}
