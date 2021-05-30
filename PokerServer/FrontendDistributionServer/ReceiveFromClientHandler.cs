using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace FrontendDistributionServer.Handlers
{
	public enum clientTOfrontend
	{
		None = 0,

		Count
	}

	public class ReceiveFromClientHandler : IReceivedMessageHandler<clientTOfrontend>
	{
		public Dictionary<clientTOfrontend, Action<UniCastPacket>> Handlers { get; } = new Dictionary<clientTOfrontend, Action<UniCastPacket>>();

		public ReceiveFromClientHandler()
		{
			Handlers.Add((int)clientTOfrontend.None, Test);
		}

		private void Test(UniCastPacket packet)
		{
			Guid id = new Guid(packet.Read(16));
			Console.WriteLine("|||" + id);
			Console.WriteLine("|||");
		}
	}
}
