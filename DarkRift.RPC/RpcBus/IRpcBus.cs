using System.Threading.Tasks;

namespace DarkRift.RPC
{
	public interface IRpcBus
	{
		Task<TResponse> Call<TRequest, TResponse>(IEndPoint endPoint, TRequest request)
			where TRequest : IDarkRiftSerializable, new()
			where TResponse : IDarkRiftSerializable, new();
	}
}