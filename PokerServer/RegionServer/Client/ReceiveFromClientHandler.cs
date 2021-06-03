﻿using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace RegionServer.Client.Handlers
{
	public enum clientTOregion
	{
		None = 0,

		Count,


		Test
	}

	public class ReceiveFromClientHandler : IReceivedMessageHandler<int>
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; } = new Dictionary<int, Action<UniCastPacket>>();

		public ReceiveFromClientHandler()
		{
			Handlers.Add((int)clientTOregion.Test, Test);
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