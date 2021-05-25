using System;
using System.Collections.Generic;

namespace UniCastCommonData.Handlers
{
	public interface ISendMessageHandler
	{
		Dictionary<int, Action<UniCastPacket>> Handlers { get; }
	}
}
