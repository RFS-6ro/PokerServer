using System;
using TestingClient.FrontendDistributionSide.Handlers;
using TestingClient.FrontendDistributionSide;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient
{
	class Program
	{
		static void Main(string[] args)
		{
			var client = new Client_FrontendDistributor("127.0.0.1", 6378);
			client.ConnectAsync();
			using (UniCastPacket packet = new UniCastPacket(ActorType.FrontendDistributionServer))
			{
				packet.Write((int)frontendTOclient.Count);

				packet.WriteLength();
				client.OnReceived(packet.GetRawBytes(), 0, 8);
			}
			//client.SendHandler.Handlers[clientTOfrontend.Count]?.Invoke(null);

			while (true)
			{
				Console.ReadLine();
			}
		}
	}
}
