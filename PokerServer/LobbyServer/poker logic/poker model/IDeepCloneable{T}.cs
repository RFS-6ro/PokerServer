namespace LobbyServer.pokerlogic
{
	public interface IDeepCloneable<out T>
	{
		T DeepClone();
	}
}
