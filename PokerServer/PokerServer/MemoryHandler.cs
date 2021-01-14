namespace PokerServer
{
	public static class MemoryHandler
	{
		public static void Clear<T>(this T[] memory, int length, int offset = 0)
		{
			for (int i = offset; i < length; i++)
			{
				memory[i] = default(T);
			}
		}

		public static T As<T>(this object @object)
		{
			return (T)@object;
		}
	}
}
