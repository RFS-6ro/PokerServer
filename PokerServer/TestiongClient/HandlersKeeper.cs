using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient.Handlers
{
	public class HandlersKeeper : IHandlersKeeper
	{
		//public Dictionary<ActorType, IReceivedMessageHandler> TypedReceiveHandlers { get; } =
		//	new Dictionary<ActorType, IReceivedMessageHandler>();

		//public Dictionary<ActorType, ISendMessageHandler> TypedSendHandlers { get; } =
		//	new Dictionary<ActorType, ISendMessageHandler>();

		//public HandlersKeeper()
		//{
		//	TypedReceiveHandlers.Add(ActorType.FrontendDistributionServer, new ReceiveFromFrontendDistributionHandler());
		//	TypedReceiveHandlers.Add(ActorType.RegionServer, new ReceiveFromRegionHandler());
		//	TypedReceiveHandlers.Add(ActorType.LobbyServer, new ReceiveFromLobbyHandler());

		//	TypedSendHandlers.Add(ActorType.FrontendDistributionServer, new SendToFrontendDistributionHandler());
		//	TypedSendHandlers.Add(ActorType.RegionServer, new SendToRegionHandler());
		//	TypedSendHandlers.Add(ActorType.LobbyServer, new SendToLobbyHandler());
		//}
	}
}
