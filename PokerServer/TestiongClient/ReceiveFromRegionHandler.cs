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

	public class ReceiveFromRegionHandler : IReceivedMessageHandler<regionTOclient>
	{
		public Dictionary<regionTOclient, Action<UniCastPacket>> Handlers { get; }

		public ReceiveFromRegionHandler()
		{
			//Handlers.Add((int)regionTOclient., (x) => { });
		}
	}
}