using System;
using System.Collections.Generic;
using System.Linq;
using UniCastCommonData.Handlers;

namespace UniCastCommonData.Network.MessageHandlers
{
	public abstract class ReceiveHandlerBase : IReceivedMessageHandler<int>
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; } = new Dictionary<int, Action<UniCastPacket>>();

		public Guid ReceiverId;

		public void Receive(int action, UniCastPacket packet)
		{
			int i = -1;
			byte[] ids = packet.Read(32, false);
			Guid senderId = new Guid(ids.TakeWhile((x) => { ++i; return (i < 16); }).ToArray());
			i = -1;
			Guid receiverId = new Guid(ids.SkipWhile((x) => { ++i; return (i < 16); }).ToArray());

			if (action == 1)
			{
				ReceiverId = receiverId;
			}

			if (ReceiverId == receiverId)
			{
				ThreadManager.ExecuteOnMainThread(() =>
				{
					Handlers[action]?.Invoke(packet);
					packet.Dispose();
				});
			}
		}
	}
}
