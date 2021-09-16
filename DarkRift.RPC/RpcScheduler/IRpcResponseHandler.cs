namespace DarkRift.RPC
{
	public interface IRpcResponseHandler
	{
		void SendResponse<TResponse>(IEndPoint endPoint, RpcWrapper<TResponse> response)
			where TResponse : IDarkRiftSerializable, new();

		void HandleResponse<TResponse>(RpcWrapper<TResponse> response)
			where TResponse : IDarkRiftSerializable, new();
	}
}
