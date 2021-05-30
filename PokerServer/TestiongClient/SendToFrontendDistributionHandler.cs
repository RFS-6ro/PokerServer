using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient.Handlers
{
	public enum clientTOfrontend
	{
		None = 0,

		Count
	}

	public class SendToFrontendDistributionHandler : ISendMessageHandler<clientTOfrontend>
	{
		public Dictionary<clientTOfrontend, Action<Guid>> Handlers { get; } = new Dictionary<clientTOfrontend, Action<Guid>>();

		public SendToFrontendDistributionHandler()
		{
			Handlers.Add(clientTOfrontend.None, Test);
		}

		private void Test(Guid id)
		{
			using (UniCastPacket packet = new UniCastPacket(Client_FrontendDistributor.Instance.ClientType))
			{
				packet.Write((int)clientTOfrontend.None);

				Console.WriteLine("|||");
				Console.WriteLine("|||" + Client_FrontendDistributor.Instance.ClientType);
				Console.WriteLine("|||" + clientTOfrontend.None);
				Console.WriteLine("|||" + id);
				packet.Write(id.ToByteArray());
				Console.WriteLine("|||" + packet.Length);
				packet.WriteLength();
				Console.WriteLine("|||");
				Client_FrontendDistributor.Instance.SendAsync(packet.ToArray());
			}
		}
	}
}