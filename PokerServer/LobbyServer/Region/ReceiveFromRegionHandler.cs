using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;

namespace LobbyServer.Region.Handlers
{
	public enum regionTOlobby
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class ReceiveFromRegionHandler : ClientReceiveHandlerBase<Lobby_Region>
	{
		public ReceiveFromRegionHandler()
		{
			Handlers.Add((int)regionTOlobby.Test, Test);
			Handlers.Add((int)regionTOlobby.Connect, Connect);
			Handlers.Add((int)regionTOlobby.Disconnect, Disconnect);
		}

		private void Disconnect(UniCastPacket packet)
		{
		}

		private void Connect(UniCastPacket packet)
		{
			Guid receiverGuid = new Guid(packet.Read(16));
			Client.SetId(receiverGuid);
		}

		private void Test(UniCastPacket packet)
		{
			Guid receiverGuid = new Guid(packet.Read(16));

			string message = packet.ReadString();

			Console.WriteLine(receiverGuid + "|" + message);
		}
	}
}