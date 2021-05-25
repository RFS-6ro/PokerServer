using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient.Handlers
{
	public class HandlersKeeper : IHandlersKeeper
	{
		public Dictionary<SenderType, IReceivedMessageHandler> TypedReceiveHandlers { get; } =
			new Dictionary<SenderType, IReceivedMessageHandler>();

		public Dictionary<SenderType, ISendMessageHandler> TypedSendHandlers { get; } =
			new Dictionary<SenderType, ISendMessageHandler>();

		public HandlersKeeper()
		{
			TypedReceiveHandlers.Add(SenderType.FrontendDistributionServer, new FrontendDistributionReceiveHandler());
			TypedReceiveHandlers.Add(SenderType.RegionServer, new RegionReceiveHandler());
			TypedReceiveHandlers.Add(SenderType.LobbyServer, new LobbyReceiveHandler());

			TypedReceiveHandlers.Add(SenderType.FrontendDistributionServer, new FrontendDistributionReceiveHandler());
			TypedReceiveHandlers.Add(SenderType.RegionServer, new RegionReceiveHandler());
			TypedReceiveHandlers.Add(SenderType.LobbyServer, new LobbyReceiveHandler());
		}
	}
}
