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
			using (UniCastPacket packet = new UniCastPacket(ActorType.Client))
			{
				packet.Write((int)clientTOfrontend.Count);

				packet.WriteLength();
				OnReceived(packet.GetRawBytes(), 0, 8);
			}
		}

		protected override void OnConnected()
		{
			Console.WriteLine("Connected");

			//FrontendDistribution_Client_Server.Instance.SendHandler.Handlers[frontendTOclient.Count]?.Invoke(Id);
		}
	}
}
