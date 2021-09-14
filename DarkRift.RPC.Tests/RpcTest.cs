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

			RpcRegister.Register<PingRequest>(0);
			RpcRegister.Register<PingResponse>(1);

			var messageFactory = new MessageFactory();
			var endPoint = new Mock<IEndPoint>();

			var scheduler = new RpcScheduler(messageFactory);

			var processor = new RpcProcessor(scheduler);

			processor.Subscribe((PingRequest r, IEndPoint ep) =>
			{
				Console.WriteLine($"Process RPC request {r} from {ep}");
				return new PingResponse() {Now = DateTime.Now};
			});

			endPoint
				.Setup(ep => ep.Send(It.IsAny<Message>(), It.IsAny<SendMode>()))
				.Callback(async (Message m, SendMode _) =>
				{
					Console.WriteLine($"Send {m}");
					await Task.Delay(TimeSpan.FromSeconds(3));
					Console.WriteLine($"Receive {m}");
					Console.WriteLine($"Process {m}");
					processor.Process(endPoint.Object, m);
				});

			var request = new PingRequest() {Now = DateTime.Now};
			Console.WriteLine($"-> Call RPC {request}");
			var response = await scheduler.Call<PingRequest, PingResponse>(endPoint.Object, request);
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
