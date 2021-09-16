using System;
using System.Collections.Generic;

namespace DarkRift.RPC
{
	public abstract class RpcDescription
	{
		public abstract bool IsRequest { get; }
		protected RpcDescription() {}

		public abstract void Deserialize(IRpcProcessor processor, IEndPoint endPoint, Message message);
	}

	public class RpcDescription<T> : RpcDescription
		where T : IDarkRiftSerializable, new()
	{
		public static ushort Tag { get; private set; }
		public static bool Initialized { get; private set; }
		public override bool IsRequest { get; }

		public RpcDescription(ushort tag, bool isRequest)
		{
			if (Initialized) throw new Exception($"RPC {typeof(T)} already registered!");
			Tag = tag;
			Initialized = true;
			IsRequest = isRequest;
		}

		public override void Deserialize(IRpcProcessor processor, IEndPoint endPoint, Message message)
		{
			processor.Process(endPoint, message.Deserialize<RpcWrapper<T>>());
		}
	}
}
