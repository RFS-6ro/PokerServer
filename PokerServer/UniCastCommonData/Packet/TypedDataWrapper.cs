namespace UniCastCommonData.Wrappers
{
	public abstract class TypedDataWrapper<T> : IBasePacketDataWrapper
	{
		protected T _data;

		public T Data => _data;
		public abstract int RawDataLength { get; }

		public TypedDataWrapper()
		{
		}

		protected TypedDataWrapper(T data)
		{
			_data = data;
		}

		public abstract byte[] GetRawBytes();
		public abstract void SetTypedData(byte[] rawData);
	}
}
