using DarkRift.Client;

namespace DarkRift.RPC.Client
{
	public class ServerEndPoint : IEndPoint
	{
		private readonly DarkRiftClient _client;

		public ServerEndPoint(DarkRiftClient client)
		{
			_client = client;
		}

		public void Send(Message message, SendMode sendMode)
		{
			_client.SendMessage(message, sendMode);
		}
	}
}
