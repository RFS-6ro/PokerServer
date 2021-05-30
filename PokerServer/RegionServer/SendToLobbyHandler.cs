using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace RegionServer.Handlers
{
	public enum regionTOlobby
	{
		None = 0,

		Count
	}

	public class SendToLobbyHandler : ISendMessageHandler<regionTOlobby>
	{
		public Dictionary<regionTOlobby, Action<UniCastPacket>> Handlers { get; }

		public SendToLobbyHandler()
		{
			//Handlers.Add((int)regionTOlobby., (x) => { });
		}
	}
}