using System;
using System.Collections.Generic;
using UniCastCommonData.Handlers;
using UniCastCommonData.Packet.InitialDatas;

namespace UniCastCommonData.Network.MessageHandlers
{
	public class ClientSender<CLIENT> : ISendMessageHandler
		where CLIENT : TcpClient, ISender
	{
		public ISender Sender { get; protected set; }

		public Dictionary<int, Action<InitialSendingData>> Handlers { get; } = new Dictionary<int, Action<InitialSendingData>>();

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
				Sender.SendAsync(packet);
			}
		}
	}
}
