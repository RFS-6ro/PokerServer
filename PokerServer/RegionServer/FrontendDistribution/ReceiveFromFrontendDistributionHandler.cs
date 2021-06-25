using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;

namespace RegionServer.FrontendDistribution.Handlers
{
	public enum frontendTOregion
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class ReceiveFromFrontendDistributionHandler : ClientReceiveHandlerBase<Region_FrontendDistribution>
	{
		public ReceiveFromFrontendDistributionHandler()
		{
			Handlers.Add((int)frontendTOregion.Test, Test);
			Handlers.Add((int)frontendTOregion.Connect, Connect);
			Handlers.Add((int)frontendTOregion.Disconnect, Disconnect);
		}

		private void Disconnect(UniCastPacket packet)
		{
		}

		private void Connect(UniCastPacket packet)
		{
			Guid receiverGuid = new Guid(packet.Read(16));
			Region_FrontendDistribution.Instance.SetId(receiverGuid);
		}

		private void Test(UniCastPacket packet)
		{
			Guid receiverGuid = new Guid(packet.Read(16));

			string message = packet.ReadString();

			Console.WriteLine(receiverGuid + "|" + message);
		}
	}
}