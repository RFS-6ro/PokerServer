﻿using System;
using UniCastCommonData.Network;

namespace LobbyServer.Client
{
	public class ClientSessionBuilder : ITCPSessionBuilder
	{
		public ClientSessionBuilder() { }

		public TcpSession Create(TcpServer server)
		{
			return new Lobby_Client_Session(server);
		}
	}
}
