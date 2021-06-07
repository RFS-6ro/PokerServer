using System;
using System.Collections.Generic;
using System.Text;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient.FrontendDistribution.Handlers
{
	public enum frontendTOclient
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class ReceiveFromFrontendDistributionHandler : IReceivedMessageHandler<int>
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; } = new Dictionary<int, Action<UniCastPacket>>();

		public ReceiveFromFrontendDistributionHandler()
		{
			Handlers.Add((int)frontendTOclient.Test, Test);
			Handlers.Add((int)frontendTOclient.Connect, Connect);
			Handlers.Add((int)frontendTOclient.Disconnect, Disconnect);
		}

		private void Disconnect(UniCastPacket packet)
		{
			ThreadManager.ExecuteOnMainThread(() =>
			{
			});
		}

		private void Connect(UniCastPacket packet)
		{
			ThreadManager.ExecuteOnMainThread(() =>
			{
				Guid senderGuid = new Guid(packet.Read(16));
				Guid receiverGuid = new Guid(packet.Read(16));
				IStaticInstance<Client_FrontendDistributor>.Instance.SetId(receiverGuid);
			});
		}

		private void Test(UniCastPacket packet)
		{
			ThreadManager.ExecuteOnMainThread(() =>
			{
				Guid guid = new Guid(packet.Read(16));

				string message = packet.ReadString();

				Console.WriteLine(guid + "|" + message);
			});
		}
	}
}