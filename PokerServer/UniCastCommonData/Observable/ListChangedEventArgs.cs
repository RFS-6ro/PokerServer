using System;
namespace UniCastCommonData.Observable
{
	/// <summary>
	/// On List Changed Event Args.
	/// </summary>
	/// <typeparam name="T">List type.</typeparam>
	public sealed class ListChangedEventArgs<T> : EventArgs
	{
		public readonly int Index;

		public readonly T Item;

		public ListChangedEventArgs(int index, T item)
		{
			Index = index;
			Item = item;
		}
	}
}
