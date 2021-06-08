using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;

namespace FrontendDistributionServer.Client.Handlers
{
	public enum clientTOfrontend
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class ReceiveFromClientHandler : SessionReceiveHandlerBase<FrontendDistribution_Client_Server>
	{
		public ReceiveFromClientHandler()
		{
			Handlers.Add((int)clientTOfrontend.Test, Test);
			Handlers.Add((int)clientTOfrontend.Connect, Connect);
			Handlers.Add((int)clientTOfrontend.Disconnect, Disconnect);
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
