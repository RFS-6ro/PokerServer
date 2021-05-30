using System;
using FrontendDistributionServer.Handlers;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace FrontendDistributionServer
{
	public class FrontendDistribution_Client_Session : TcpSession
	{
		public FrontendDistribution_Client_Session(TcpServer server) : base(server)
		{
		}

		protected override void OnConnected()
		{
			Console.WriteLine("Connected");

			FrontendDistribution_Client_Server.Instance.SendHandler.Handlers[frontendTOclient.Count]?.Invoke(Id);
		}

		protected override void OnReceived(byte[] buffer, long offset, long size)
		{
			using (UniCastPacket packet = new UniCastPacket(buffer))
			{
				Console.WriteLine("|||");
				int length = packet.ReadInt();
				Console.WriteLine("|||" + length);

				ActorType actor = (ActorType)packet.ReadInt();
				Console.WriteLine("|||" + actor);

				if (actor != FrontendDistribution_Client_Server.Instance.ClientType)
				{
					return;
				}

				clientTOfrontend action = (clientTOfrontend)packet.ReadInt();
				Console.WriteLine("|||" + action);

				FrontendDistribution_Client_Server.Instance.ReceiveHandler.Handlers[action]?.Invoke(packet);

			}
		}
	}
}
