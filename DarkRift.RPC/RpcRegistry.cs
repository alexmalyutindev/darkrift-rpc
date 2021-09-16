using System;
using System.Collections.Generic;

namespace DarkRift.RPC
{
	public static class RpcRegistry
	{
		private static readonly Dictionary<ushort, RpcDescription> Descriptions =
			new Dictionary<ushort, RpcDescription>();

		public static ushort GetTag<T>() where T : IDarkRiftSerializable, new() => RpcDescription<T>.Tag;

		public static void RegisterRequest<T>(ushort tag) where T : IDarkRiftSerializable, new()
		{
			if (RpcDescription<T>.Initialized) throw new Exception($"RPC {typeof(T)} already registered!");
			Descriptions.Add(tag, new RpcDescription<T>(tag, true));
		}

		public static void RegisterResponse<T>(ushort tag) where T : IDarkRiftSerializable, new()
		{
			if (RpcDescription<T>.Initialized) throw new Exception($"RPC {typeof(T)} already registered!");
			Descriptions.Add(tag, new RpcDescription<T>(tag, false));
		}

		public static bool TryGetDescription(ushort tag, out RpcDescription value) => Descriptions.TryGetValue(tag, out value);
	}
}
