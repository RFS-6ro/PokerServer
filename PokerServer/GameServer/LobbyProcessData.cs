using System;
using System.Diagnostics;
using Network;

namespace GameServer
{
	public class LobbyProcessData : INeedLogger
	{
		public readonly Process Process;

		public LoggerBase _logger => ConsoleLogger.Instance;

		public bool IsResponsable => Process.Responding;

		public LobbyProcessData(string name, string args)
		{
			try
			{
				Process process = new Process();
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.FileName = "/Users/RFS_6ro/Documents/GitHub/PokerServer/PokerServer/PokerLobby/bin/Release/net5.0/osx.10.12-x64/PokerLobby";

				if (args != null)
				{
					process.StartInfo.Arguments = args;
				}

				if (process.Start())
				{
					Process = process;
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
	}
}
