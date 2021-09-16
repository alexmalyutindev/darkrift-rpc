using System;
using System.Collections.Generic;

namespace DarkRift.RPC
{
	public class RpcMessageSink
	{
		private readonly IRpcResponseHandler _responseHandler;
		private readonly Dictionary<ushort, IRpcSubscriber> _requestSubscribers = new Dictionary<ushort, IRpcSubscriber>();

		private readonly IRpcProcessor _requestProcessor;
		private readonly IRpcProcessor _responseProcessor;

		public RpcMessageSink(IRpcResponseHandler responseHandler)
		{
			_responseHandler = responseHandler;
			_requestProcessor = new RpcRequestProcessor(this);
			_responseProcessor = new RpcResponseProcessor(this);
		}

		public void Subscribe<TRequest, TResponse>(Func<TRequest, TResponse> func)
			where TRequest : IDarkRiftSerializable, new()
			where TResponse : IDarkRiftSerializable, new()
		{
			Subscribe<TRequest, TResponse>((request, _) => func(request));
		}

		public void Subscribe<TRequest, TResponse>(Func<TRequest, IEndPoint, TResponse> func)
			where TRequest : IDarkRiftSerializable, new()
			where TResponse : IDarkRiftSerializable, new()
		{
			if (_requestSubscribers.ContainsKey(RpcRegistry.GetTag<TRequest>()))
				throw new Exception($"RPC Request {typeof(TRequest)} was already subscribed");
			_requestSubscribers.Add(RpcRegistry.GetTag<TRequest>(), new RequestSubscriber<TRequest, TResponse>(func));
		}

		public void Unsubscribe<TRequest, TResponse>(Func<TRequest, TResponse> func)
			where TRequest : IDarkRiftSerializable, new()
			where TResponse : IDarkRiftSerializable, new()
		{
			if (!_requestSubscribers.ContainsKey(RpcRegistry.GetTag<TRequest>()))
				throw new Exception($"RPC Request {typeof(TRequest)} has no subscriber");
			_requestSubscribers.Remove(RpcRegistry.GetTag<TRequest>());
		}

		public void HandleMessage(IEndPoint endPoint, Message message)
		{
			if (RpcRegistry.TryGetDescription(message.Tag, out var description))
			{
				if (description.IsRequest)
				{
					if (_requestSubscribers.TryGetValue(message.Tag, out var subscriber))
						subscriber.Invoke(_requestProcessor, endPoint, message);
					else
						throw new Exception(
							$"RPC request {description.GetType().GenericTypeArguments[0]} has no subscription!");
				}
				else
				{
					description.Deserialize(_responseProcessor, endPoint, message);
				}
			}
		}

		public void SendResponse<TResponse>(IEndPoint endPoint, RpcWrapper<TResponse> response)
			where TResponse : IDarkRiftSerializable, new()
		{
			_responseHandler.SendResponse(endPoint, response);
		}

		public void HandleResponse<TResponse>(RpcWrapper<TResponse> response)
			where TResponse : IDarkRiftSerializable, new()
		{
			_responseHandler.HandleResponse(response);
		}
	}
}
