using System;
using UniCastCommonData;

namespace TestingClient
{
	public class PokerPlayerConsoleInput : ConsoleInput<TestingClientMediator>
	{
		public PokerPlayerConsoleInput(TestingClientMediator mediator) : base(mediator) { }

		protected override void checkInput(string input)
		{
			base.checkInput(input);

			if (input.Contains("Name = "))
			{
				string name = input.Split(' ')[2];
				_mediator.SetName(name);
				return;
			}

			//TODO: check all actions here

			InputModel.SetTurn(input);
		}
	}
}
