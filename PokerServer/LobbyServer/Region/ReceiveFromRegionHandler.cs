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

	public class ReceiveFromRegionHandler : ReceiveHandlerBase
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
			Guid senderGuid = new Guid(packet.Read(16));
			Guid receiverGuid = new Guid(packet.Read(16));
			IStaticInstance<Lobby_Region>.Instance.SetId(receiverGuid);
		}

		private void Test(UniCastPacket packet)
		{
			Guid senderGuid = new Guid(packet.Read(16));
			Guid receiverGuid = new Guid(packet.Read(16));

			string message = packet.ReadString();

			Console.WriteLine(senderGuid + "|" + message);
		}
	}
}