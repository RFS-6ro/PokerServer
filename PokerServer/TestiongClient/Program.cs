using System;
using System.Net;
using TestingClient.Handlers;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

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
			return;
			byte[] vector = { 146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 221, 112, 79, 32, 114, 156 };
			byte[] vector2 = { 146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 221, 112, 79, 32, 114, 156 };

			for (int i = 0; i < vector.Length; i++)
			{
				for (int j = vector.Length / 4; j < vector.Length / 2; j++)
				{
					vector2[i] = (byte)(vector[i] ^ vector[j]);
				}
			}

			for (int i = 0; i < vector2.Length; i++)
			{
				for (int j = vector2.Length / 4; j < vector2.Length / 2; j++)
				{
					vector[i] = (byte)(vector2[i] ^ vector2[j]);
				}
			}

			Console.ReadKey();
			Console.ReadKey();
		}
	}
}
