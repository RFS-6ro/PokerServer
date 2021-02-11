using System;
using System.Diagnostics;

namespace GameServer
{
	public class LobbyNetworkBunch
	{
		public int ID { get; set; }
		public LobbyClient Client { get; set; }

		public string Name { get; set; }
		public int SmallBlind { get; set; }
		public int BuyIn { get; set; }
		public Process Process { get; set; }
		public bool IsAssigned { get; private set; }

		public LobbyNetworkBunch(int id, LobbyClient client)
		{
			ID = id;
			Client = client ?? throw new ArgumentNullException(nameof(client));
		}

		public void AssignNewLobby(string name, int smallBlind, int buyIn, Process process)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			SmallBlind = smallBlind;
			BuyIn = buyIn;
			Process = process ?? throw new ArgumentNullException(nameof(process));
			IsAssigned = true;
		}

		public void ResetLobby()
		{
			IsAssigned = false;
			Name = null;
			SmallBlind = 0;
			BuyIn = -1;
			Process = null;
		}
	}
}
