﻿using ServerPool;

namespace RegionServer.Lobby
{
	internal class LobbyServerProcessFactory : IServerProcessFactory<Lobby_Server_Process, string[]>
	{
		public Lobby_Server_Process CreateWithParams(string[] param)
		{
			return new Lobby_Server_Process(param);
		}
	}
}
