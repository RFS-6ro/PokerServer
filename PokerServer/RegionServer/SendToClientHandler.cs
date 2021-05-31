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

	public class SendToClientHandler : ISendMessageHandler<InitialSendingData>
	{
		public Dictionary<InitialSendingData, Action<InitialSendingData>> Handlers { get; } = new Dictionary<InitialSendingData, Action<InitialSendingData>>();

		public SendToClientHandler()
		{
			//Handlers.Add((int)regionTOclient., (x) => { });
		}
	}
}
