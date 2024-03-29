﻿using System;
using System.Diagnostics;
using System.Text;
using Network;
using PokerSynchronisation;

namespace GameServer
{
	public class LobbyProcessData : INeedLogger
	{
		public readonly Process Process;

		public LoggerBase _logger => ConsoleLogger.Instance;

		public bool IsResponsable => Process.Responding;

		public LobbyIdentifierData LobbyIdentifierData { get; private set; }

		public LobbyProcessData(LobbyIdentifierData data, string[] args)
		{
			try
			{
				Process process = new Process();
				process.StartInfo.CreateNoWindow = false;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.FileName = "/Users/RFS_6ro/Documents/GitHub/PokerServer/PokerServer/PokerLobby/bin/Debug/net5.0/osx.10.12-x64/PokerLobby";

				string linedAttributes = data.ToString();

				if (args != null)
				{
					linedAttributes += GenerateLinedString(args);
				}

				process.StartInfo.Arguments = linedAttributes;

				if (process.Start())
				{
					Process = process;
					LobbyIdentifierData = data;
				}
				else
				{
					_logger.PrintError($"Error with starting lobby process.");
				}
			}
			catch (Exception ex)
			{
				_logger.PrintError($"Lobby creation throws exception {ex}.");
			}

		}

		private string GenerateLinedString(string[] args)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < args.Length; i++)
			{
				builder.Append(' ');
				builder.Append(args[i]);
			}

			return builder.ToString();
		}
	}
}
