
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
			StaticLogger.Print("initializing server with " + address);
		}

		protected override TcpSession CreateSession()
		{
			StaticLogger.Print("creating session for new connection");
			return new Lobby_Client_Session(this);
		}

		public override bool Start()
		{
			StaticLogger.Print("starting server");
			return base.Start();
		}

		public override bool Stop()
		{
			StaticLogger.Print("stopping server");
			return base.Stop();
		}

		public override bool Restart()
		{
			StaticLogger.Print("restarting server");
			return base.Restart();
		}

		public override bool DisconnectAll()
		{
			StaticLogger.Print("disconnect all users");
			return base.DisconnectAll();
		}

		public override bool Multicast(byte[] buffer)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < buffer.Length; i++)
			{
				stringBuilder.Append(buffer[i].ToString());
				stringBuilder.Append(" ");
			}

			StaticLogger.Print(new object[]
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
				stringBuilder.Append(" ");
			}

			StaticLogger.Print(new object[]
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
			StaticLogger.Print(new object[]
			{
				"multicasting for all users",
				text
			});
			return base.Multicast(text);
		}

		protected override void OnDisconnecting(TcpSession session)
		{
			StaticLogger.Print(new object[]
			{
				"session is trying to disconnect",
				$"Id = {session.Id}",
				$"Socket = {session.Socket}"
			});
			base.OnDisconnecting(session);
		}

		protected override void OnDisconnected(TcpSession session)
		{
			StaticLogger.Print(new object[]
			{
				"session was disconnected",
				$"Id = {session.Id}",
				$"Socket = {session.Socket}"
			});
			base.OnDisconnected(session);
		}

		protected override void OnError(SocketError error)
		{
			StaticLogger.Print(new object[]
			{
				"Socket Error",
				$"Error = {error}"
			});
			base.OnError(error);
		}

		protected override void Dispose(bool disposingManagedResources)
		{
			StaticLogger.Print("Disposing");
			base.Dispose(disposingManagedResources);
		}

		protected override void OnConnecting(TcpSession session)
		{
			StaticLogger.Print(new object[]
			{
				"new session is trying to connect",
				$"Id = {session.Id}",
				$"Socket = {session.Socket}"
			});
			base.OnConnecting(session);
		}

		protected override void OnConnected(TcpSession session)
		{
			base.OnConnected(session);
			StaticLogger.Print(new object[]
			{
				"new session was connected",
				$"new Id = {session.Id}",
				$"Socket = {session.Socket}"
			});
		}
	}
}
