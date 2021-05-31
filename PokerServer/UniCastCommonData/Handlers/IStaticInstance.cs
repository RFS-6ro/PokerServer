namespace UniCastCommonData
{
	public interface IStaticInstance<T>
	{
		static T Instance { get; protected set; }
	}
}
