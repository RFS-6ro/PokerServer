using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace FrontendDistributionServer.Database
{
	public enum databaseTOfrontend
	{
		None = 0,

		Count,


		Test
	}

	public class ReceiveFromDatabaseHandler : IReceivedMessageHandler<int>
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; } = new Dictionary<int, Action<UniCastPacket>>();

		public ReceiveFromDatabaseHandler()
		{
			Handlers.Add((int)databaseTOfrontend.Test, Test);
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
