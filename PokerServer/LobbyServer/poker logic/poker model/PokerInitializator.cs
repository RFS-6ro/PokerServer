using GameCore.Handlers;
using GameCore.Poker.Controller;
using GameCore.Poker.Model;
using GameCore.Poker.Model.Player;
using GameCore.Poker.ViewModel;
using GameCore.UI;
using System.Collections;
using System.Collections.Generic;
using TexasHoldem.Logic.Players;
using UnityEngine;

public class PokerInitializator : InitiableMonoBehaviour
{
	public TableViewModel TableViewModel;

	public InputController InputController;

	public List<ChairViewModel> Chairs;

	public List<TextView> BotNames;

	public void SignalButton(string text)
	{
		Debug.Log(text);
	}

	public override void Init(object[] initParams = null)
	{
		int playerCount = 9;
		int randomPlayerCount = Random.Range(2, playerCount);
		int shiftNumber = Random.Range(0, randomPlayerCount - 1);
		Debug.Log(randomPlayerCount);
		Debug.Log(shiftNumber);

		List<IPlayer> players = new List<IPlayer>();
		List<UnityPlayerDecorator> decorators = new List<UnityPlayerDecorator>();

		for (int i = 0; i < playerCount; i++)
		{
			IPlayer player;
			UnityPlayerDecorator decorator;
			if (i == 0)
			{
				player = new RealPlayer();
				decorator = new RealPlayerDecorator(Chairs[0], InputController);
			}
			else
			{
				player = new BotPlayer();
				decorator = new BotPlayerDecorator(Chairs[i], BotNames[i]);
			}

			decorator.SetPlayer(player);
			players.Add(player);
			decorators.Add(decorator);
		}

		for (int i = 0; i < playerCount - randomPlayerCount; i++)
		{
			int randomPlayerIndex = Random.Range(1, players.Count);
			players.RemoveAt(randomPlayerIndex);
			decorators.RemoveAt(randomPlayerIndex);
		}

		for (int i = 0; i <= shiftNumber; i++)
		{
			players.Add(players[0]);
			players.RemoveAt(0);
			decorators.Add(decorators[0]);
			decorators.RemoveAt(0);
		}

		for (int i = 0; i < decorators.Count; i++)
		{
			decorators[i].ConfigureSeat();

			if (decorators[i].GetType() == typeof(BotPlayerDecorator))
			{
				((BotPlayerDecorator)decorators[i]).NameHolder.Show(decorators[i].Name);
			}

		}
		//chairs.RemoveRange(shiftNumber, playerCount - randomPlayerCount);

		//int index = 0;

		//for (int i = playerCount - shiftNumber + 1; i >= 1; i--)
		//{
		//	UnityPlayerDecorator predDecorator = new BotPlayerDecorator(chairs[i], BotNames[i]);

		//	predDecorator.SetPlayer(players[index]);
		//	decorators.Add(predDecorator);
		//	index++;
		//}

		//UnityPlayerDecorator mainOlayerDecorator = new RealPlayerDecorator(chairs[0], null);

		//mainOlayerDecorator.SetPlayer(players[index]);
		//decorators.Add(mainOlayerDecorator);

		//for (int i = 1; i < randomPlayerCount - shiftNumber; i++)
		//{
		//	UnityPlayerDecorator predDecorator = new BotPlayerDecorator(chairs[i], BotNames[i]);

		//	predDecorator.SetPlayer(players[index]);
		//	decorators.Add(predDecorator);
		//	index++;
		//}


		var game = new TexasHoldemGame<UnityPlayerDecorator>(decorators, TableViewModel);
		StartCoroutine(StartGame(game));
	}

	private IEnumerator StartGame<TDECORATOR>(TexasHoldemGame<TDECORATOR> game) where TDECORATOR : UnityPlayerDecorator, new()
	{
		TDECORATOR winner;

		yield return game.Start((x) => winner = x);
	}
}
