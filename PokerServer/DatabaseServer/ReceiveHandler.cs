using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;

namespace DatabaseServer
{
	public enum anyTOdatabase
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class ReceiveHandler : SessionReceiveHandlerBase<Database_Server>
	{
		public ReceiveHandler()
		{
			Handlers.Add((int)anyTOdatabase.Test, Test);
			Handlers.Add((int)anyTOdatabase.Connect, Connect);
			Handlers.Add((int)anyTOdatabase.Disconnect, Disconnect);
		}

		private void Disconnect(UniCastPacket packet)
		{
		}

		private void Connect(UniCastPacket packet)
		{
		}

		private void Test(UniCastPacket packet)
		{
			Guid receiverGuid = new Guid(packet.Read(16));

			string message = packet.ReadString();

			Console.WriteLine(receiverGuid + "|" + message);
		}
	}
}