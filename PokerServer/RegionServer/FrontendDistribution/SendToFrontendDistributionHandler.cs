﻿using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace RegionServer.FrontendDistribution.Handlers
{
	public enum regionTOfrontend
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class SendToFrontendDistributionHandler : ISendMessageHandler<int>
	{
		public ISender Sender { get; set; }

		public Dictionary<int, Action<InitialSendingData>> Handlers { get; } = new Dictionary<int, Action<InitialSendingData>>();

		public SendToFrontendDistributionHandler()
		{
			Handlers.Add((int)regionTOfrontend.Test, Test);
			Handlers.Add((int)regionTOfrontend.Connect, Connect);
			Handlers.Add((int)regionTOfrontend.Disconnect, Disconnect);
		}

		private void Disconnect(InitialSendingData data)
		{
			using (UniCastPacket packet = new UniCastPacket(data))
			{


				Sender.SendAsync(packet);
			}
		}

		private void Connect(InitialSendingData data)
		{
			using (UniCastPacket packet = new UniCastPacket(data))
			{


				Sender.SendAsync(packet);
			}
		}

		private void Test(InitialSendingData data)
		{
			using (UniCastPacket packet = new UniCastPacket(data))
			{
				packet.Write(GetType().ToString());

				Sender.SendAsync(packet);
			}
		}
	}
}
