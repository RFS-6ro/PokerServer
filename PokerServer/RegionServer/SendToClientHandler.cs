using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace RegionServer.Handlers
{
	public enum regionTOclient
	{
		None = 0,

		Count
	}

	public class SendToClientHandler : ISendMessageHandler<regionTOclient>
	{
		public Dictionary<regionTOclient, Action<UniCastPacket>> Handlers { get; }

		public SendToClientHandler()
		{
			//Handlers.Add((int)regionTOclient., (x) => { });
		}
	}
}
