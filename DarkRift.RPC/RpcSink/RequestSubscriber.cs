using System;

namespace DarkRift.RPC
{
	public class RequestSubscriber<TRequest, TResponse> : IRpcSubscriber
		where TRequest : IDarkRiftSerializable, new()
		where TResponse : IDarkRiftSerializable, new()
	{
		private readonly Func<TRequest, IEndPoint, TResponse> _func;

		public RequestSubscriber(Func<TRequest, IEndPoint, TResponse> func) => _func = func;

		public void Invoke(RpcProcessor processor, IEndPoint endPoint, Message message)
		{
			var request = message.Deserialize<RpcWrapper<TRequest>>();
			var response = _func(request.Value, endPoint);
			var rpc = new RpcWrapper<TResponse>(request.ID, response);
			processor.SendResponse(endPoint, rpc);
		}
	}
}
