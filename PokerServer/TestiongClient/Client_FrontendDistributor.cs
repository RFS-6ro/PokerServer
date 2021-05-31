using System;
using System.Net;
using UniCastCommonData.Network;
using TestingClient.Handlers;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient
{
	public class Client_FrontendDistributor : AbstractTCPClient<
		ReceiveFromFrontendDistributionHandler, int,
		SendToFrontendDistributionHandler, int,
		Client_FrontendDistributor>
	{
		public override ActorType ServerType => ActorType.FrontendDistributionServer;

		public override ActorType ClientType => ActorType.Client;

		public Client_FrontendDistributor(IPAddress address, int port) : base(address, port)
		{
		}

		public Client_FrontendDistributor(string address, int port) : base(address, port)
		{
		}

		protected override void OnConnected()
		{
			Console.WriteLine("Connected");

			//SendHandler.Handlers[clientTOfrontend.None]?.Invoke(Id);
		}

		public override bool OnReceived(byte[] buffer, long offset, long size)
		{
			return base.OnReceived(buffer, offset, size);
			using (UniCastPacket packet = new UniCastPacket(buffer))
			{
				int length = packet.ReadInt();

				Console.WriteLine(length);

				ActorType type = (ActorType)packet.ReadInt();
				Console.WriteLine(type);

				int action = packet.ReadInt();
				Console.WriteLine(action);

				ReceiveHandler.Handlers[action]?.Invoke(packet);


			}
		}
	}
}
