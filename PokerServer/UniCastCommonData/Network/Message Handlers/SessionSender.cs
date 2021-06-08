using System;
using System.Collections.Generic;
using UniCastCommonData.Handlers;
using UniCastCommonData.Packet.InitialDatas;

namespace UniCastCommonData.Network.MessageHandlers
{
	public abstract class SessionSender<SERVER> : ISendMessageHandler<int>
		where SERVER : TcpServer, IStaticInstance<SERVER>
	{
		public SERVER Server => IStaticInstance<SERVER>.Instance;

		public ISender Sender { get; set; }

		public Dictionary<int, Action<InitialSendingData>> Handlers { get; } = new Dictionary<int, Action<InitialSendingData>>();

		public ISender GetSenderByID(Guid guid)
		{
			Sender = Server.FindSession(guid);
			return Sender;
		}

		protected void SendAsync(InitialSendingData data, byte[][] content)
		{
			using (UniCastPacket packet = new UniCastPacket(data))
			{
				if (content != null)
				{
					foreach (var item in content)
					{
						packet.Write(item);
					}
				}

				packet.WriteLength();
				GetSenderByID(data.ReceiverGuid).SendAsync(packet);
			}
		}
	}
}
