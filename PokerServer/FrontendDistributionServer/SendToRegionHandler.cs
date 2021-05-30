using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace FrontendDistributionServer.Handlers
{
	public enum frontendTOregion
	{
		None = 0,

		Count
	}

	public class SendToRegionHandler : ISendMessageHandler<frontendTOregion>
	{
		public Dictionary<frontendTOregion, Action<Guid>> Handlers { get; }

		public SendToRegionHandler()
		{
			//Handlers.Add((int)frontendTOregion., (x) => { });
		}
	}
}
