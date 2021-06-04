using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace DatabaseServer
{
	public enum anyTOdatabase
	{
		None = 0,

		Count,


		Test
	}

	public class ReceiveHandler : IReceivedMessageHandler<int>
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; } = new Dictionary<int, Action<UniCastPacket>>();

		public ReceiveHandler()
		{
			Handlers.Add((int)anyTOdatabase.Test, Test);
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