namespace DarkRift.RPC
{
	public class RpcRequestProcessor : IRpcProcessor
	{
		private readonly RpcMessageSink _sink;

		public RpcRequestProcessor(RpcMessageSink sink) => _sink = sink;

		public void Process<T>(IEndPoint endPoint, RpcWrapper<T> data) where T : IDarkRiftSerializable, new()
		{
			_sink.SendResponse(endPoint, data);
		}
	}
}
