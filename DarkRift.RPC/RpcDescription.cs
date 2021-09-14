using System;

namespace DarkRift.RPC
{
	public class RpcDescription
	{
		protected RpcDescription() {}

		public static ushort GetTag<T>() where T : IDarkRiftSerializable
		{
			return RpcDescription<T>.Tag;
		}
	}

	public class RpcDescription<T> : RpcDescription
		where T : IDarkRiftSerializable
	{
		public static bool Initialized { get; private set; }
		public static ushort Tag { get; private set; }
		public RpcDescription(ushort tag)
		{
			if (Initialized) throw new Exception($"RPC {typeof(T)} already registered!");
			Tag = tag;
			Initialized = true;
		}
	}
}
