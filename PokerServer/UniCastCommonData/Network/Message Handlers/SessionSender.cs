using System;
using System.Collections.Generic;
using UniCastCommonData.Handlers;

namespace UniCastCommonData.Network.MessageHandlers
{
	public abstract class SessionSender<SERVER> : ISendMessageHandler<int>
		where SERVER : TcpServer, IStaticInstance<SERVER>
	{
		public SERVER Server { get; }

		public ISender Sender { get; set; }

		public Dictionary<int, Action<InitialSendingData>> Handlers { get; } = new Dictionary<int, Action<InitialSendingData>>();

		protected SessionSender()
		{
			Server = IStaticInstance<SERVER>.Instance;
		}

		public ISender GetSenderByID(Guid guid)
		{
			Sender = Server.FindSession(guid);
			return Sender;
		}
	}
}
