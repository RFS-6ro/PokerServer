using System;
using UniCastCommonData.Handlers;

namespace UniCastCommonData
{
	public class InitialSendingData : IByteArrayConvertable
	{
		private ActorType _actorType;
		public ActorType ActorType => _actorType;

		private int _action;
		public int Action => _action;

		private Guid _guid;
		public Guid Guid => _guid;

		public InitialSendingData(Guid guid, ActorType actorType, int action)
		{
			_actorType = actorType;
			_action = action;
			_guid = guid;
		}

		public byte[] GetRawBytes()
		{
			byte[] data = new byte[24];

			((int)_actorType).ToByteArray().CopyTo(data, 0);
			_action.ToByteArray().CopyTo(data, 4);
			_guid.ToByteArray().CopyTo(data, 8);

			return data;
		}

		public override string ToString()
		{
			return _guid.ToString();
		}
	}
}
