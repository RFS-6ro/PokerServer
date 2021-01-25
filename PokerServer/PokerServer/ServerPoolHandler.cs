using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Net.NetworkInformation;
using Network;
using System.Net;

namespace FrontendServer
{
	public static class ServerPoolHandler
	{
		private static List<ServerIdentifierData> _allRegisteredServers = new List<ServerIdentifierData>
		{
			new ServerIdentifierData(NetworkSettings.LOCAL_HOST_IP, NetworkSettings.TEST_SERVERPORT)
		};
		private static List<ServerIdentifierData> _currentRunningServers = new List<ServerIdentifierData>();

		public static void RegisterresponsibleServers()
		{

		}

		public static void CheckServersResponseability()
		{
			//_currentRunningServers.RemoveAll((x) => IsNotResponseable(x));
			//_currentRunningServers.AddRange(_allRegisteredServers.Where((x, y) => IsNotResponseable(x) == false));
		}

		public static ServerIdentifierData GetClosestServer(int clientId)
		{
			return _allRegisteredServers[0];
		}

		public static void IsNotResponseable(ServerIdentifierData serverIdentifier)
		{
			try
			{
				//new Ping().SendAsync(serverIdentifier.IpAddress, 1000, );
			}
			catch
			{
			}
		}

		public static void I(ServerIdentifierData serverIdentifier)
		{

		}
	}
}
