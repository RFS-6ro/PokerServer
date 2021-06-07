﻿using System;
using UniCastCommonData;
using UniCastCommonData.Network.MessageHandlers;

namespace TestingClient.Region.Handlers
{
	public enum regionTOclient
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
			Handlers.Add((int)regionTOclient.Test, Test);
			Handlers.Add((int)regionTOclient.Connect, Connect);
			Handlers.Add((int)regionTOclient.Disconnect, Disconnect);
		}

		private void Disconnect(UniCastPacket packet)
		{
		}

		private void Connect(UniCastPacket packet)
		{
			Guid senderGuid = new Guid(packet.Read(16));
			Guid receiverGuid = new Guid(packet.Read(16));
			IStaticInstance<Client_Region>.Instance.SetId(receiverGuid);
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