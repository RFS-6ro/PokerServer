using System;
using System.Collections.Generic;
using LobbyServer.pokerlogic.Cards;

namespace LobbyServer.pokerlogic.controllers
{
	public class TableViewModel
	{
		#region Dealer

		public void MoveDealerButton(Guid to)
		{

		}

		#endregion

		#region Cards 

		public List<Card> Cards { get; private set; } = new List<Card>();

		public void ClearCards()
		{
		}

		#endregion
	}
}
