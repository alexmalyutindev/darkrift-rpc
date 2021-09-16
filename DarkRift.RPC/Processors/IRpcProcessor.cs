namespace DarkRift.RPC
{
	public interface IRpcProcessor
	{
		void Process<T>(IEndPoint endPoint, RpcWrapper<T> data) where T : IDarkRiftSerializable, new();
	}
}
