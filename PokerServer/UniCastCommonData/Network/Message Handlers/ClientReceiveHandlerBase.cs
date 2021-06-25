using System;
using System.Collections.Generic;
using UniCastCommonData.Handlers;

namespace UniCastCommonData.Network.MessageHandlers
{
	public abstract class ClientReceiveHandlerBase<CLIENT> : IReceivedMessageHandler
		where CLIENT : TcpClient
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; } = new Dictionary<int, Action<UniCastPacket>>();

		public Guid ReceiverId;

		public virtual void Receive(int action, UniCastPacket packet, Guid guid)
		{
			ThreadManager.ExecuteOnMainThread(() =>
			{
				Guid senderId = new Guid(packet.Read(16));
				Guid receiverId = new Guid(packet.Read(16));
				packet.Reset(32);

				if (action == 1 && receiverId != Guid.Empty)
				{
					ReceiverId = receiverId;
				}

				if (receiverId == Guid.Empty || ReceiverId == receiverId)
				{
					Handlers[action]?.Invoke(packet);
					packet.Dispose();
				}
			});
		}
	}
}
