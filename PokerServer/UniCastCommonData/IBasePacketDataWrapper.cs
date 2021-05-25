namespace UniCastCommonData.Wrappers
{
	public interface IBasePacketDataWrapper
	{
		int RawDataLength { get; }

		byte[] GetRawData();

		void SetTypedData(byte[] rawData);
	}
}
