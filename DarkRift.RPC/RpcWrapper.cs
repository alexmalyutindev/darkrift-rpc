using System;

namespace DarkRift.RPC
{
	public sealed class RpcWrapper<T> : IDarkRiftSerializable where T : IDarkRiftSerializable, new()
	{
		public ulong ID;
		public T Value;

		public RpcWrapper() { }

		public RpcWrapper(ulong id, T value)
		{
			ID = id;
			Value = value;
		}

		void IDarkRiftSerializable.Serialize(SerializeEvent e)
		{
			e.Writer.Write(ID);
			Value.Serialize(e);
		}

		void IDarkRiftSerializable.Deserialize(DeserializeEvent e)
		{
			ID = e.Reader.ReadUInt64();
			Value = e.Reader.ReadSerializable<T>(); // Requires default ctor
		}
	}
}
