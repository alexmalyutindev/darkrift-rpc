namespace DarkRift.RPC
{
	public interface IRpcSubscriber
	{
		void Invoke(IRpcProcessor processor, IEndPoint endPoint, Message message);
	}
}
