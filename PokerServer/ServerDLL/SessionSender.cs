using System;
using System.Collections.Generic;
using UniCastCommonData.Handlers;
using UniCastCommonData.Packet.InitialDatas;

namespace UniCastCommonData.Network.MessageHandlers
{
	public abstract class SessionSender<SERVER> : ISendMessageHandler
		where SERVER : TcpServer
	{
		public SERVER Server { get; set; }

		public ISender Sender { get; set; }

		public Dictionary<int, Action<InitialSendingData>> Handlers { get; } = new Dictionary<int, Action<InitialSendingData>>();

		protected ISender GetSenderByID(Guid guid)
		{
			Sender = Server.FindSession(guid);
			return Sender;
		}

		public void SendAsync(InitialSendingData data, byte[][] content)
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
				GetSenderByID(data.ReceiverGuid)?.SendAsync(packet);
			}
		}

		public void Multicast(IEnumerable<Guid> clients, InitialSendingData data, byte[][] content)
		{
			data.ReceiverGuid = Guid.Empty;

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

				foreach (var clientGuid in clients)
				{
					GetSenderByID(clientGuid)?.SendAsync(packet);
				}
			}
		}

		public void MulticastExept(IEnumerable<Guid> clients, InitialSendingData data, byte[][] content, Guid guid)
		{
			data.ReceiverGuid = Guid.Empty;

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

				foreach (var clientGuid in clients)
				{
					if (clientGuid == guid)
					{
						continue;
					}
					GetSenderByID(clientGuid)?.SendAsync(packet);
				}
			}
		}
	}
}
