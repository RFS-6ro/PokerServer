using System;
using System.Collections.Generic;
using Network;

namespace GameServer
{
	public class LobbyPoolhandler : SingletonBase<LobbyPoolhandler>
	{
		private Dictionary<string, LobbyProcessData> _lobbiesContainer = new Dictionary<string, LobbyProcessData>();

		public LobbyProcessData GetLobbyByName(string lobbyName, string args = null)
		{
			if (_lobbiesContainer.ContainsKey(lobbyName) == false)
			{
				return CreateNewLobby(lobbyName, args);
			}

			LobbyProcessData lobbyData = _lobbiesContainer[lobbyName];

			if (lobbyData.IsResponsable == false)
			{
				throw new Exception("Lobby is not responding");
			}

			return lobbyData;
		}

		private LobbyProcessData CreateNewLobby(string lobbyName, string args = null)
		{
			LobbyProcessData lobbyData = new LobbyProcessData(lobbyName, args);
			_lobbiesContainer.Add(lobbyName, lobbyData);

			return lobbyData;
		}
	}
}
