using System;
using System.Collections.Generic;

namespace UniCastCommonData.Handlers
{
	public interface IReceivedMessageHandler
	{
		Dictionary<int, Action<UniCastPacket>> Handlers { get; }
	}
}
