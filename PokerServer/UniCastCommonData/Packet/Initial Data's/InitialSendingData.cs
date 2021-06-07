using System;
using UniCastCommonData.Handlers;

namespace UniCastCommonData.Packet.InitialDatas
{
	public class InitialSendingData : IByteArrayConvertable
	{
		private ActorType _actorType;
		public ActorType ActorType => _actorType;

		private int _action;
		public int Action => _action;

		private Guid _senderGuid;
		public Guid SenderGuid => _senderGuid;

		private Guid _receiverGuid;
		public Guid ReceiverGuid => _receiverGuid;

		public InitialSendingData(Guid receiverGuid, Guid senderGuid, ActorType actorType, int action)
		{
			_actorType = actorType;
			_action = action;
			_senderGuid = senderGuid;
			_receiverGuid = receiverGuid;
		}

		public virtual byte[] GetRawBytes()
		{
			byte[] data = new byte[40];

			((int)_actorType).ToByteArray().CopyTo(data, 0);
			_action.ToByteArray().CopyTo(data, 4);
			_senderGuid.ToByteArray().CopyTo(data, 8);
			_receiverGuid.ToByteArray().CopyTo(data, 24);

			return data;
		}
	}
}
