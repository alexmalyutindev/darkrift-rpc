namespace DarkRift.RPC
{
	public interface IEndPoint
	{
		// TODO: Add other necessary data
		void Send(Message message, SendMode sendMode);
	}
}
