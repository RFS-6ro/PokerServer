using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using PokerSynchronisation;

namespace GameServer
{
	public class LobbyPoolhandler : SingletonBase<LobbyPoolhandler>
	{
		private Dictionary<LobbyIdentifierData, LobbyProcessData> _lobbiesContainer = new Dictionary<LobbyIdentifierData, LobbyProcessData>();

		public LobbyProcessData GetLobbyByName(string name, string[] args = null)
		{
			LobbyIdentifierData data = _lobbiesContainer.FirstOrDefault((pair) => pair.Key.Name == name).Key;
			if (data != null)
			{
				return CreateNewLobby(data, args);
			}

			LobbyProcessData lobbyData = _lobbiesContainer[data];

			if (lobbyData.IsResponsable == false)
			{
				throw new Exception("Lobby is not responding");
			}

			return lobbyData;
		}

		public LobbyProcessData CreateNewLobby(LobbyIdentifierData data, string[] args = null)
		{
			LobbyProcessData lobbyData = new LobbyProcessData(data, args);
			_lobbiesContainer.Add(data, lobbyData);

			return lobbyData;
		}
	}
}
