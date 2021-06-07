using System;
using System.Collections.Generic;
using System.Text;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;

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

	public class ReceiveFromFrontendDistributionHandler : ReceiveHandlerBase
	{
		public ReceiveFromFrontendDistributionHandler()
		{
			Handlers.Add((int)frontendTOclient.Test, Test);
			Handlers.Add((int)frontendTOclient.Connect, Connect);
			Handlers.Add((int)frontendTOclient.Disconnect, Disconnect);
		}

		private void Disconnect(UniCastPacket packet)
		{
		}

		private void Connect(UniCastPacket packet)
		{
			Guid senderGuid = new Guid(packet.Read(16));
			Guid receiverGuid = new Guid(packet.Read(16));
			IStaticInstance<Client_FrontendDistributor>.Instance.SetId(receiverGuid);
		}

		private void Test(UniCastPacket packet)
		{
			Guid senderGuid = new Guid(packet.Read(16));
			Guid receiverGuid = new Guid(packet.Read(16));

			string message = packet.ReadString();

			Console.WriteLine(senderGuid + "|" + message);
		}
	}
}