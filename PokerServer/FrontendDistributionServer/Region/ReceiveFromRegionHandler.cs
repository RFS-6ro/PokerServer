using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;

namespace FrontendDistributionServer.Region.Handlers
{
	public enum regionTOfrontend
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class ReceiveFromRegionHandler : SessionReceiveHandlerBase<FrontendDistribution_Region_Server>
	{
		public ReceiveFromRegionHandler()
		{
			Handlers.Add((int)regionTOfrontend.Test, Test);
			Handlers.Add((int)regionTOfrontend.Connect, Connect);
			Handlers.Add((int)regionTOfrontend.Disconnect, Disconnect);
		}

		private void Disconnect(UniCastPacket packet)
		{
		}

		private void Connect(UniCastPacket packet)
		{
		}

		private void Test(UniCastPacket packet)
		{
			Guid receiverGuid = new Guid(packet.Read(16));

			string message = packet.ReadString();

			Console.WriteLine(receiverGuid + "|" + message);
		}
	}
}
