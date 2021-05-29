using System;
using System.Net.Sockets;
using UniCastCommonData.Network;
using UniCastCommonData;

namespace FrontendDistributionServer
{
	public class FrontendDistribution_Session : TcpSession
	{
		public FrontendDistribution_Session(TcpServer server) : base(server)
		{
		}

		public override long Receive(byte[] buffer, long offset, long size)
		{
			long value = base.Receive(buffer, offset, size);

			return value;
		}

		public override bool SendAsync(byte[] buffer, long offset, long size)
		{
			return base.SendAsync(buffer, offset, size);
		}

		protected override void OnConnected()
		{
			Console.WriteLine($"Player with Id {Id} connected!");

			// Send invite message
			string message = "Hello from TCP Frontend Distribution Server";
			SendAsync(message);
		}

		protected override void OnConnecting()
		{
			Console.WriteLine($"Player with Id {Id} is connecting to Frontend Distribution Server!");
			base.OnConnecting();
		}

		protected override void OnDisconnecting()
		{
			Console.WriteLine($"Player with Id {Id} is disconnecting from Frontend Distribution Server!");
			base.OnDisconnecting();
		}

		protected override void OnError(SocketError error)
		{
			Console.WriteLine($"Chat TCP session caught an error with code {error}");
		}
	}
}
