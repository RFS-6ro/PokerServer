using System;
using UniCastCommonData.Handlers;

namespace UniCastCommonData
{
	public class InitialSendingData : IByteArrayConvertable
	{
		private Guid _guid;
		private ActorType _actorType;
		private int _action;

		public Guid Guid => _guid;
		public ActorType ActorType => _actorType;
		public int Action => _action;

		public InitialSendingData(Guid guid, ActorType actorType, int action)
		{
			_guid = guid;
			_actorType = actorType;
			_action = action;
		}

		public byte[] GetRawBytes()
		{
			byte[] data = new byte[24];

			_guid.ToByteArray().CopyTo(data, 0);
			((int)_actorType).ToByteArray().CopyTo(data, 16);
			_action.ToByteArray().CopyTo(data, 20);

			return data;
		}

		public override string ToString()
		{
			return _guid.ToString();
		}
	}
}
