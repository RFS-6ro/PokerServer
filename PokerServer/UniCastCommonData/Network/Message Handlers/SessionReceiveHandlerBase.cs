﻿using System;
using System.Collections.Generic;
using UniCastCommonData.Handlers;

namespace UniCastCommonData.Network.MessageHandlers
{
	public class SessionReceiveHandlerBase<SERVER> : IReceivedMessageHandler<int>
		where SERVER : TcpServer, IStaticInstance<SERVER>
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; } = new Dictionary<int, Action<UniCastPacket>>();

		public SERVER Server => IStaticInstance<SERVER>.Instance;

		public Guid LastSenderGuid { get; protected set; }

		public void Receive(int action, UniCastPacket packet, Guid guid)
		{
			ThreadManager.ExecuteOnMainThread(() =>
			{
				Guid senderId = new Guid(packet.Read(16));
				packet.Reset(16);

				LastSenderGuid = action == 1 ? guid : senderId;

				Handlers[action]?.Invoke(packet);
				packet.Dispose();
			});
		}
	}
}
