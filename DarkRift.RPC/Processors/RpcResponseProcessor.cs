using System;

namespace DarkRift.RPC
{
	public class RpcResponseProcessor : IRpcProcessor
	{
		private readonly RpcMessageSink _sink;

		public RpcResponseProcessor(RpcMessageSink sink) => _sink = sink;

		public void Process<T>(IEndPoint endPoint, RpcWrapper<T> data) where T : IDarkRiftSerializable, new()
		{
			_sink.HandleResponse(data);
		}
	}
}
