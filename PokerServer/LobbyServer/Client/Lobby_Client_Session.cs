using System.Net.Sockets;
using System.Text;
using ServerDLL;
using UniCastCommonData;
using UniCastCommonData.Network;

namespace LobbyServer.Client
{
	public class Lobby_Client_Session : TcpSession
	{
		public string Name { get; }

		public Lobby_Client_Session(TcpServer server) : base(server) { }

		protected override void OnDisconnecting()
		{
			StaticLogger.Print($"Lobby_Client_Session + {Id.ToString().Split('-')[0]}", "disconnecting");
			PokerInitializator.Instance.RemovePlayer(Id);
		}

		public override bool Disconnect()
		{
			StaticLogger.Print($"Lobby_Client_Session + {Id.ToString().Split('-')[0]}", "event to disconnect");
			return base.Disconnect();
		}

		public override bool SendAsync(UniCastPacket packet)
		{
			StaticLogger.Print($"Lobby_Client_Session + {Id.ToString().Split('-')[0]}",
				new string[]
				{
					"Sending packet to user",
					packet.ToString()
				}
			);
			return base.SendAsync(packet);
		}

		protected override void OnConnecting()
		{
			StaticLogger.Print($"Lobby_Client_Session + {Id.ToString().Split('-')[0]}",
				new string[]
				{
					"new user is trying to connect",
					$"Id = {Id}"
				}
			);
			base.OnConnecting();
		}

		protected override void OnConnected()
		{
			StaticLogger.Print($"Lobby_Client_Session + {Id.ToString().Split('-')[0]}",
				new string[]
				{
					"new user was connected",
					$"Id = {Id}"
				}
			);
			base.OnConnected();
		}

		protected override void OnDisconnected()
		{
			StaticLogger.Print($"Lobby_Client_Session + {Id.ToString().Split('-')[0]}", "was successfully disconnected");
			base.OnDisconnected();
		}

		protected override bool OnReceived(byte[] buffer, long offset, long size)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < buffer.Length; i++)
			{
				stringBuilder.Append(buffer[i].ToString());
				if (i % 4 == 0) stringBuilder.Append(" ");
			}

			StaticLogger.Print($"Lobby_Client_Session + {Id.ToString().Split('-')[0]}",
				new string[]
				{
					"Receiving packet from user",
					stringBuilder.ToString()
				}
			);

			return base.OnReceived(buffer, offset, size);
		}

		protected override void OnEmpty()
		{
			StaticLogger.Print($"Lobby_Client_Session + {Id.ToString().Split('-')[0]}", "packet is empty");
			base.OnEmpty();
		}

		protected override void OnError(SocketError error)
		{
			StaticLogger.Print($"Lobby_Client_Session + {Id.ToString().Split('-')[0]}", $"Socket Error occured: {error}");
			base.OnError(error);
		}
	}
}
