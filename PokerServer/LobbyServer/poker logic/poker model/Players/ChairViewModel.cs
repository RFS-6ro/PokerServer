using System;
using System.Collections.Generic;
using LobbyServer.pokerlogic.Cards;

namespace LobbyServer.pokerlogic.pokermodel.Players
{
	public class ChairViewModel
	{
		public void StartTurn()
		{
		}

		public void EndTurn()
		{
		}

		#region Bet

		#endregion

		#region Cards

		public List<Card> Cards { get; private set; } = new List<Card>();

		public void ShowCards()
		{
		}

		public void ClearCards()
		{
		}

		#endregion

		#region Timer

		public void StartTimer(float amount)
		{
		}

		public void UpdateTimer()
		{
		}

		public void StopTimer()
		{
		}

		#endregion

		#region Game State

		public void SetGameStateHolder(string state)
		{
		}

		#endregion

		#region Money View

		public void SetMoneyView(int amount)
		{
		}

		#endregion
	}
}
