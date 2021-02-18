using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameServer
{
	public class LobbyNetworkBunch
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public int SmallBlind { get; set; }
		public int BuyIn { get; set; }
		public LobbyClient Client { get; set; }
		public LobbyProcessData Process { get; set; }
		public bool IsAssigned { get; private set; }
		public List<int> CurrentPlayersIDs => Client?.RegisteredPlayers;

		public LobbyNetworkBunch(int id, LobbyClient client)
		{
			ID = id;
			Client = client ?? throw new ArgumentNullException(nameof(client));
		}

		public void AssignNewLobby(string name, int smallBlind, int buyIn, LobbyProcessData process)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			SmallBlind = smallBlind;
			BuyIn = buyIn;
			//Process = process ?? throw new ArgumentNullException(nameof(process));
			IsAssigned = true;
		}

		public void ResetLobby()
		{
			IsAssigned = false;
			Name = null;
			if (CurrentPlayersIDs != null)
			{
				foreach (var playerId in CurrentPlayersIDs)
				{
					MainGameServerSendsToPlayerHandle.DisconnectFromLobby(playerId);
				}
			}
			Client?.Disconnect();
			SmallBlind = 0;
			BuyIn = -1;
			Process = null;
		}
	}
}
