using System;
namespace UniCastCommonData.Observable
{
	/// <summary>
	/// On List Changed Event Args.
	/// </summary>
	/// <typeparam name="T">List type.</typeparam>
	public sealed class ConcurrentDictionaryChangedEventArgs<TKey, TValue> : EventArgs
	{
		public readonly TKey Key;

		public readonly TValue Value;

		public ConcurrentDictionaryChangedEventArgs(TKey key, TValue value)
		{
			Key = key;
			Value = value;
		}
	}
}
