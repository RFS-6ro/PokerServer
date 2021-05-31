using System;
using System.Collections.Generic;
using System.Text;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient.Handlers
{
	public enum frontendTOclient
	{
		None = 0,

		Count
	}

	public class ReceiveFromFrontendDistributionHandler : IReceivedMessageHandler<int>
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; } = new Dictionary<int, Action<UniCastPacket>>();

		public ReceiveFromFrontendDistributionHandler()
		{
			Handlers.Add((int)frontendTOclient.Count, Test);
		}

		private void Test(UniCastPacket packet)
		{
			Console.WriteLine("!@#$#@!@#$#@!");
			return;
			Guid id = new Guid(packet.Read(16));
			Console.WriteLine(id);

			int length = packet.ReadInt(); // Get the length of the string
			string value = Encoding.ASCII.GetString(packet.Read(length), 0, length);

			Console.WriteLine(value);

		}
	}
}