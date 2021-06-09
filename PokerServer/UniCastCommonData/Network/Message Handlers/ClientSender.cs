using System;
using System.Collections.Generic;
using UniCastCommonData.Handlers;
using UniCastCommonData.Packet.InitialDatas;

namespace UniCastCommonData.Network.MessageHandlers
{
	public class ClientSender<CLIENT> : ISendMessageHandler<int>
		where CLIENT : TcpClient, ISender, IStaticInstance<CLIENT>
	{
		public ISender Sender => IStaticInstance<CLIENT>.Instance;

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
