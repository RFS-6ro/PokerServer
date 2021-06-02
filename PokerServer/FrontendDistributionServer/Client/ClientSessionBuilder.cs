﻿using System;
using UniCastCommonData.Network;

namespace FrontendDistributionServer.Client
{
	public class ClientSessionBuilder : ITCPSessionBuilder
	{
		public ClientSessionBuilder() { }

		public TcpSession Create(TcpServer server)
		{
			return new FrontendDistribution_Client_Session(server);
		}
	}
}
