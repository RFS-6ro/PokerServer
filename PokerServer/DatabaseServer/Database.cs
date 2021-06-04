using System;
using System.Collections.Generic;

namespace DatabaseServer
{
	[Serializable]
	public class Database
	{
		public List<Player> Players = new List<Player>();

		public void AddPlayer(Player player)
		{
			if (player == null)
			{
				return;
			}

			Players.Add(player);
		}
	}

	[Serializable]
	public class Player
	{
		public string Login;
		public string Password;
		public string Device;
		public int Balance;
	}
}
