using DatabaseServer;
using UniCastCommonData;
using UniCastCommonData.Network.MessageHandlers;

namespace DatabaseServer
{
	public enum frontendTOregion
	{
		None = 0,

		Count,


		Test
	}

	public class SendHandler : SessionSender<Database_Server>
	{
		public SendHandler()
		{
			Handlers.Add((int)frontendTOregion.Test, Test);
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