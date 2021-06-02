using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace RegionServer.Client.Handlers
{
	public enum regionTOclient
	{
		None = 0,

		Count
	}

	public class SendToClientHandler : ISendMessageHandler<int>
	{
		public Dictionary<int, Action<InitialSendingData>> Handlers { get; } = new Dictionary<int, Action<InitialSendingData>>();

		public SendToClientHandler()
		{
			//Handlers.Add((int)regionTOclient., (x) => { });
		}
	}
}
