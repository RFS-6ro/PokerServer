using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient.Handlers
{
	public enum regionTOclient
	{
		None = 0,

		Count
	}

	public class RegionReceiveHandler : IReceivedMessageHandler
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; }

		public RegionReceiveHandler()
		{
			//Handlers.Add((int)regionTOclient., (x) => { });
			//Handlers.Add((int)regionTOclient., (x) => { });
			//Handlers.Add((int)regionTOclient., (x) => { });
			//Handlers.Add((int)regionTOclient., (x) => { });
		}
	}
}