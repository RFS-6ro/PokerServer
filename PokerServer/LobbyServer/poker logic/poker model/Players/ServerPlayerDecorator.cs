using System;
namespace LobbyServer.pokerlogic.pokermodel.Players
{
	public class ServerPlayerDecorator : UnityPlayerDecorator
	{
		public ServerPlayerDecorator(ChairViewModel chairView) : base(chairView)
		{
		}
	}
}
