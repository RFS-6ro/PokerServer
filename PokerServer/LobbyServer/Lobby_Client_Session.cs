using System;
using LobbyServer.Handlers;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace LobbyServer
{
	public class Lobby_Client_Session : TcpSession
	{
		public Lobby_Client_Session(TcpServer server) : base(server)
		{
		}



		protected override void OnConnected()
		{
			base.OnConnected();
		}

		protected override void OnConnecting()
		{
			base.OnConnecting();
		}



		protected override void OnReceived(byte[] buffer, long offset, long size)
		{
			using (UniCastPacket packet = new UniCastPacket(buffer))
			{
				int length = packet.ReadInt();

				if (((ActorType)packet.ReadInt()) != Lobby_Client_Server.Instance.ClientType)
				{
					return;
				}

				clientTOlobby action = (clientTOlobby)packet.ReadInt();

				Lobby_Client_Server.Instance.ReceiveHandler.Handlers[action]?.Invoke(packet);

			}
		}



		protected override void OnDisconnected()
		{
			base.OnDisconnected();
		}

		protected override void OnDisconnecting()
		{
			base.OnDisconnecting();
		}
	}
}
