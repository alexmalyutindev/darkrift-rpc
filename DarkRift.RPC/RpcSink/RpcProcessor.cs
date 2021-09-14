using System;
using System.Collections.Generic;

namespace DarkRift.RPC
{
	public class RpcProcessor
	{
		private readonly RpcScheduler _scheduler;
		private readonly Dictionary<ushort, IRpcSubscriber> _subscribers = new Dictionary<ushort, IRpcSubscriber>();

		public RpcProcessor(RpcScheduler scheduler)
		{
			_scheduler = scheduler;
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
			if (_subscribers.ContainsKey(RpcDescription.GetTag<TRequest>()))
				throw new Exception($"RPC Request {typeof(TRequest)} was already subscribed");
			if (_subscribers.ContainsKey(RpcDescription.GetTag<TRequest>()))
				throw new Exception($"RPC Response {typeof(TRequest)} was already subscribed");

			_subscribers.Add(RpcDescription.GetTag<TRequest>(), new RequestSubscriber<TRequest, TResponse>(func));
			_subscribers.Add(RpcDescription.GetTag<TResponse>(), new ResponseSubscriber<TResponse>());
		}

		public void Unsubscribe<TRequest, TResponse>(Func<TRequest, TResponse> func)
			where TRequest : IDarkRiftSerializable
			where TResponse : IDarkRiftSerializable
		{
			if (!_subscribers.ContainsKey(RpcDescription.GetTag<TRequest>()))
				throw new Exception($"RPC Request {typeof(TRequest)} has no subscriber");
			if (!_subscribers.ContainsKey(RpcDescription.GetTag<TRequest>()))
				throw new Exception($"RPC Response {typeof(TRequest)} has no subscriber");

			_subscribers.Remove(RpcDescription.GetTag<TRequest>());
			_subscribers.Remove(RpcDescription.GetTag<TResponse>());
		}

		public void Process(IEndPoint endPoint, Message message)
		{
			if (_subscribers.TryGetValue(message.Tag, out var subscriber))
				subscriber.Invoke(this, endPoint, message);
			else
			{
				// TODO: Logger
			}
		}

		public void SendResponse<TResponse>(IEndPoint endPoint, RpcWrapper<TResponse> response)
			where TResponse : IDarkRiftSerializable, new()
		{
			_scheduler.SendResponse(endPoint, response);
		}

		public void HandleResponse<TResponse>(RpcWrapper<TResponse> response)
			where TResponse : IDarkRiftSerializable, new()
		{
			_scheduler.HandleResponse(response);
		}
	}
}
