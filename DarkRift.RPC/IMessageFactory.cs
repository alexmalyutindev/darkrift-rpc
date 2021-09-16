namespace DarkRift.RPC
{
	public interface IMessageFactory
	{
		Message Create<T>(ushort tag, T message) where T : IDarkRiftSerializable;
	}
}
