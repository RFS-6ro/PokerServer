using System;
using System.Collections.Generic;
using System.Text;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace FrontendDistributionServer.Handlers
{
	public enum frontendTOclient
	{
		None = 0,

		Count
	}

	public class SendToClientHandler : ISendMessageHandler<frontendTOclient>
	{
		public Dictionary<frontendTOclient, Action<Guid>> Handlers { get; } = new Dictionary<frontendTOclient, Action<Guid>>();

		public SendToClientHandler()
		{
			Handlers.Add(frontendTOclient.Count, Test);
		}

		private void Test(Guid id)
		{
			using (UniCastPacket packet = new UniCastPacket(FrontendDistribution_Client_Server.Instance.ServerType))
			{
				frontendTOclient action = frontendTOclient.Count;
				Console.WriteLine(action);
				packet.Write((int)action);
				packet.Write(id.ToByteArray());
				Console.WriteLine(id);

				string message = "Test message";
				packet.Write(message.Length); // Add the length of the string to the packet
				packet.Write(Encoding.ASCII.GetBytes(message)); // Add the string itself
				Console.WriteLine(message);

				Console.WriteLine(packet.Length);
				packet.WriteLength();

				FrontendDistribution_Client_Server.Instance.FindSession(id).SendAsync(packet.ToArray());
			}
		}
	}
}
