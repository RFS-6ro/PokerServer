
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using LobbyServer.Client.Handlers;
using ServerDLL;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace LobbyServer.Client
{
	public class Lobby_Client_Server : AbstractTCPServer<
		ReceiveFromClientHandler,
		SendToClientHandler,
		Lobby_Client_Server>
	{
		public override ActorType ServerType => ActorType.LobbyServer;

		public override ActorType ClientType => ActorType.Client;

		public Lobby_Client_Server(IPAddress address, int port) : base(address, port)
		{
			StaticLogger.Print("Lobby_Client_Server", "initializing server with " + address);
		}

		protected override TcpSession CreateSession()
		{
			StaticLogger.Print("Lobby_Client_Server", "creating session for new connection");
			return new Lobby_Client_Session(this);
		}

		public override bool Start()
		{
			StaticLogger.Print("Lobby_Client_Server", "starting server");
			return base.Start();
		}

		public override bool Stop()
		{
			StaticLogger.Print("Lobby_Client_Server", "stopping server");
			return base.Stop();
		}

		public override bool Restart()
		{
			StaticLogger.Print("Lobby_Client_Server", "restarting server");
			return base.Restart();
		}

		public override bool DisconnectAll()
		{
			StaticLogger.Print("Lobby_Client_Server", "disconnect all users");
			return base.DisconnectAll();
		}

		public override bool Multicast(byte[] buffer)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < buffer.Length; i++)
			{
				stringBuilder.Append(buffer[i].ToString());
				if (i % 4 == 0) stringBuilder.Append(" ");
			}

			StaticLogger.Print("Lobby_Client_Server", new string[]
			{
				"multicasting for all users",
				stringBuilder.ToString()
			});
			return base.Multicast(buffer);
		}

		public override bool Multicast(byte[] buffer, long offset, long size)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < buffer.Length; i++)
			{
				stringBuilder.Append(buffer[i].ToString());
				if (i % 4 == 0) stringBuilder.Append(" ");
			}

			StaticLogger.Print("Lobby_Client_Server", new string[]
			{
				"multicasting for all users",
				stringBuilder.ToString(),
				$"offset = {offset}",
				$"size = {size}"
			});
			return base.Multicast(buffer, offset, size);
		}

		public override bool Multicast(string text)
		{
			StaticLogger.Print("Lobby_Client_Server", new string[]
			{
				"multicasting for all users",
				text
			});
			return base.Multicast(text);
		}

		protected override void OnDisconnecting(TcpSession session)
		{
			StaticLogger.Print("Lobby_Client_Server", new string[]
			{
				"session is trying to disconnect",
				$"Id = {session.Id}"
			});
			base.OnDisconnecting(session);
		}

		protected override void OnDisconnected(TcpSession session)
		{
			StaticLogger.Print("Lobby_Client_Server", new string[]
			{
				"session was disconnected",
				$"Id = {session.Id}"
			});
			base.OnDisconnected(session);
		}

		protected override void OnError(SocketError error)
		{
			StaticLogger.Print("Lobby_Client_Server", $"Socket Error occured: {error}");
			base.OnError(error);
		}

		protected override void Dispose(bool disposingManagedResources)
		{
			StaticLogger.Print("Lobby_Client_Server", "Disposing");
			base.Dispose(disposingManagedResources);
		}

		protected override void OnConnecting(TcpSession session)
		{
			StaticLogger.Print("Lobby_Client_Server", new string[]
			{
				"new session is trying to connect",
				$"Id = {session.Id}"
			});
			base.OnConnecting(session);
		}

		protected override void OnConnected(TcpSession session)
		{
			base.OnConnected(session);
			StaticLogger.Print("Lobby_Client_Server", new string[]
			{
				"new session was connected",
				$"new Id = {session.Id}"
			});
		}
	}
}
