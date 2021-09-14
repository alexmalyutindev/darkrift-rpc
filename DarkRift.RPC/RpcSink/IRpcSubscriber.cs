namespace DarkRift.RPC
{
	public interface IRpcSubscriber
	{
		void Invoke(RpcProcessor processor, IEndPoint endPoint, Message message);
	}
}
