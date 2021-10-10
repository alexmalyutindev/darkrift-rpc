using System;

namespace DarkRift.RPC
{
	public class RequestSubscriber<TRequest, TResponse, TSender> : IRpcSubscriber<TSender>
		where TRequest : IDarkRiftSerializable, new()
		where TResponse : IDarkRiftSerializable, new()
		where TSender : IEndPoint
	{
		private readonly Func<TSender, TRequest, TResponse> _func;

		public RequestSubscriber(Func<TSender, TRequest, TResponse> func) => _func = func;

		public void Invoke(IRpcProcessor processor, TSender sender, Message message)
		{
			var request = message.Deserialize<RpcWrapper<TRequest>>();
			var response = _func(sender, request.Value);
			var rpc = new RpcWrapper<TResponse>(request.ID, response);
			processor.Process(sender, rpc);
		}
	}
}
