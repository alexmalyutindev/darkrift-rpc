namespace DarkRift.RPC
{
	public class ResponseSubscriber<TResponse> : IRpcSubscriber
		where TResponse : IDarkRiftSerializable, new()
	{
		public void Invoke(RpcProcessor processor, IEndPoint endPoint, Message message)
		{
			var response = message.Deserialize<RpcWrapper<TResponse>>();
			processor.HandleResponse(response);
		}
	}
}
