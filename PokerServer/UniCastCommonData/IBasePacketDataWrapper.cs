namespace UniCastCommonData.Wrappers
{
	public interface IBasePacketDataWrapper : IByteArrayConvertable
	{
		int RawDataLength { get; }

		void SetTypedData(byte[] rawData);
	}
}
