using System.Collections.Generic;

namespace DarkRift.RPC
{
	public static class RpcRegister
	{
		private static List<RpcDescription> descriptions = new List<RpcDescription>();

		public static void Register<T>(ushort tag) where T : IDarkRiftSerializable
		{
			descriptions.Add(new RpcDescription<T>(tag));
		}
	}
}
