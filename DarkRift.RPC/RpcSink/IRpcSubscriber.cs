namespace DarkRift.RPC
{
	public interface IRpcSubscriber<in TSender> where TSender : IEndPoint
	{
		void Invoke(IRpcProcessor processor, TSender endPoint, Message message);
	}
}
