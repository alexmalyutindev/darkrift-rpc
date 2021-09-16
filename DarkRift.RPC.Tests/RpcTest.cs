using System;
using System.Threading.Tasks;
using DarkRift.Client;
using NUnit.Framework;
using Moq;

namespace DarkRift.RPC.Tests
{
	public class Tests
	{
		[SetUp]
		public void Setup() { }

		[Test]
		public async Task RpcCallTest()
		{
			var objectCacheHelper = new ObjectCacheHelper();
			objectCacheHelper.InitializeObjectCache(ClientObjectCacheSettings.DontUseCache);

			RpcRegistry.RegisterRequest<PingRequest>(0);
			RpcRegistry.RegisterResponse<PingResponse>(1);

			var messageFactory = new MessageFactory();
			var server = new Mock<IEndPoint>();
			var client = new Mock<IEndPoint>();

			// Client
			var clientScheduler = new RpcScheduler(messageFactory);
			var clientSink = new RpcMessageSink(clientScheduler);

			// Server
			var serverScheduler = new RpcScheduler(messageFactory);
			var severSink = new RpcMessageSink(serverScheduler);

			server.Setup(ep => ep.Send(It.IsAny<Message>(), It.IsAny<SendMode>()))
				.Callback(async (Message m, SendMode _) =>
				{
					Console.WriteLine($"[Client] Send {m}");
					await Task.Delay(TimeSpan.FromMilliseconds(100));
					Console.WriteLine($"[Server] Receive {m}");
					Console.WriteLine($"[Server] Process {m}");
					severSink.HandleMessage(client.Object, m);
				});

			client.Setup(ep => ep.Send(It.IsAny<Message>(), It.IsAny<SendMode>()))
				.Callback(async (Message m, SendMode _) =>
				{
					Console.WriteLine($"[Server] Send {m}");
					await Task.Delay(TimeSpan.FromMilliseconds(100));
					Console.WriteLine($"[Client] Receive {m}");
					Console.WriteLine($"[Client] Process {m}");
					clientSink.HandleMessage(server.Object, m);
				});

			severSink.Subscribe((PingRequest r, IEndPoint ep) =>
			{
				Console.WriteLine($"[Server] Call subscriber for RPC request {r} from {ep}");
				return new PingResponse() {Now = DateTime.Now};
			});

			var request = new PingRequest() {Now = DateTime.Now};
			Console.WriteLine($"-> Call RPC {request}");

			var response = await clientScheduler.Call<PingRequest, PingResponse>(server.Object, request);
			Console.WriteLine($"<- Receive RPC response {response}");

			Assert.Pass();
		}

		public class MessageFactory : IMessageFactory
		{
			public Message Create<T>(ushort tag, T message) where T : IDarkRiftSerializable
			{
				return Message.Create(tag, message);
			}
		}

		public class PingRequest : IDarkRiftSerializable
		{
			public DateTime Now;
			public void Deserialize(DeserializeEvent e) => Now = new DateTime(e.Reader.ReadInt64());
			public void Serialize(SerializeEvent e) => e.Writer.Write(Now.Ticks);
			public override string ToString() => $"PingRequest {Now}";
		}

		public class PingResponse : IDarkRiftSerializable
		{
			public DateTime Now;
			public void Deserialize(DeserializeEvent e) => Now = new DateTime(e.Reader.ReadInt64());
			public void Serialize(SerializeEvent e) => e.Writer.Write(Now.Ticks);
			public override string ToString() => $"PingResponse {Now}";
		}
	}
}
