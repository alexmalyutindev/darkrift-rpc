using DarkRift;
using DarkRift.RPC;
using DarkRift.Server;

namespace RPC.Server
{
	public class ClientEndPoint : IEndPoint
	{
		private readonly IClient _client;

		public ClientEndPoint(IClient client)
		{
			_client = client;
		}

		public void Send(Message message, SendMode sendMode)
		{
			_client.SendMessage(message, sendMode);
		}
	}
}
