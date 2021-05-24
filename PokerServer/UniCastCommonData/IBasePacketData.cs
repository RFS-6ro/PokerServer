namespace UniCastCommonData
{
	public abstract class IBasePacketData<T>
	{
		private T _data;

		public IBasePacketData(T data) { _data = data; }

		public abstract byte[] GetRawData(T data);

		public abstract T GetTypedData(byte[] rawData);
	}
}
