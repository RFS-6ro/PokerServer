﻿using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient.Handlers
{
	public enum clientTOregion
	{
		None = 0,

		Count
	}

	public class SendToRegionHandler : ISendMessageHandler<clientTOregion>
	{
		public Dictionary<clientTOregion, Action<UniCastPacket>> Handlers { get; }

		public SendToRegionHandler()
		{
			//Handlers.Add((int)clientTOregion., (x) => { });
		}
	}
}