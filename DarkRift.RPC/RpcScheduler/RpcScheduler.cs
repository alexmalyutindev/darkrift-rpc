using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DarkRift.RPC
{
	public class RpcScheduler
	{
		private ulong _rpcCounter;
		private readonly IMessageFactory _messageFactory;
		private readonly Dictionary<ulong, Delegate> _requestStack = new Dictionary<ulong, Delegate>();

		public RpcScheduler(IMessageFactory messageFactory)
		{
			_messageFactory = messageFactory;
		}

		public async Task<TResponse> Call<TRequest, TResponse>(IEndPoint endPoint, TRequest request)
			where TRequest : IDarkRiftSerializable, new()
			where TResponse : IDarkRiftSerializable, new()
		{
			var requestRpc = new RpcWrapper<TRequest>(GetRequestId(), request);
			var message = _messageFactory.Create(RpcDescription.GetTag<TRequest>(), requestRpc);

			var tcs = new TaskCompletionSource<TResponse>();
			var handler = new Action<TResponse>(response => tcs.SetResult(response));

			_requestStack.Add(requestRpc.ID, handler);

			endPoint.Send(message, SendMode.Reliable);

			return await tcs.Task;
		}

		public void SendResponse<TResponse>(IEndPoint endPoint, RpcWrapper<TResponse> response)
			where TResponse : IDarkRiftSerializable, new()
		{
			var message = _messageFactory.Create(RpcDescription.GetTag<TResponse>(), response);
			endPoint.Send(message, SendMode.Reliable);
		}

		public void HandleResponse<TResponse>(RpcWrapper<TResponse> response)
			where TResponse : IDarkRiftSerializable, new()
		{
			if (_requestStack.TryGetValue(response.ID, out var handler) && handler is Action<TResponse> action)
			{
				action(response.Value);
				_requestStack.Remove(response.ID);
			}
			else
				throw new Exception($"RPC response with id {response.ID} was not awaited!");
		}

		private ulong GetRequestId()
		{
			// TODO: Uniq id throw network.
			return _rpcCounter++;
		}
	}
}
