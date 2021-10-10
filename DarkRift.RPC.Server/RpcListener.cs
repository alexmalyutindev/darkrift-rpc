using System;
using DarkRift.Server;

namespace DarkRift.RPC
{
	public class RpcListener : NetworkListener
	{
		public RpcListener(NetworkListenerLoadData pluginLoadData) : base(pluginLoadData) { }
		public override Version Version { get; }
		public override void StartListening()
		{

		}
	}
}
