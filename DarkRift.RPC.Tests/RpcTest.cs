using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DarkRift.Client;
using DarkRift.Server;
using NUnit.Framework;
using Moq;
using RPC.Server;
using MessageReceivedEventArgs = DarkRift.Server.MessageReceivedEventArgs;

namespace DarkRift.RPC.Tests
{
	public class Tests
	{
		private ObjectCacheHelper _objectCacheHelper;
		private TestEnv _server;
		private TestEnv _client;

		[SetUp]
		public void Setup()
		{
			_objectCacheHelper = new ObjectCacheHelper();
			_objectCacheHelper.InitializeObjectCache(ClientObjectCacheSettings.DontUseCache);

			_server = TestEnv.Create("Server");
			_client = TestEnv.Create("Client");
		}

		[Test]
		public async Task RpcCallTest()
		{
			RpcRegistry.RegisterRequest<PingRequest>(0);
			RpcRegistry.RegisterResponse<PingResponse>(1);

			_server.EndPointMock.Setup(ep => ep.Send(It.IsAny<Message>(), It.IsAny<SendMode>()))
				.Callback(async (Message m, SendMode _) =>
				{
					_client.Log($"Send {m}");
					await Task.Delay(TimeSpan.FromMilliseconds(100));
					_server.Log($"Receive {m}");
					_server.Sink.HandleMessage(_client.EndPointMock.Object, m);
				});

			_client.EndPointMock.Setup(ep => ep.Send(It.IsAny<Message>(), It.IsAny<SendMode>()))
				.Callback(async (Message m, SendMode _) =>
				{
					_server.Log($"Send {m}");
					await Task.Delay(TimeSpan.FromMilliseconds(100));
					_client.Log($"Receive {m}");
					_client.Sink.HandleMessage(_server.EndPointMock.Object, m);
				});

			_server.Sink.Subscribe((IEndPoint sender, PingRequest req) =>
			{
				var res = new PingResponse() {Now = DateTime.Now};
				_server.Log($"Resolve RPC request [{req}] with response [{res}]");
				return res;
			});

			var request = new PingRequest() {Now = DateTime.Now};
			_client.Log($"Call RPC [{request}]");
			var response = await _client.RpcBus.Call<PingRequest, PingResponse>(_server.EndPointMock.Object, request);
			_client.Log($"Receive RPC response [{response}]");

			Assert.Pass();
		}

		[TearDown]
		public void TearDown() { }

		private class TestEnv
		{
			public string Name { get; set; }
			public Mock<IEndPoint> EndPointMock { get; set; }
			public Mock<IMessageFactory> MessageFactory { get; set; }
			public RpcBus RpcBus { get; set; }
			public RpcMessageSink<IEndPoint> Sink { get; set; }

			public static TestEnv Create(string name)
			{
				var factory = new Mock<IMessageFactory>();
				factory
					.Setup(f =>
						f.Create(It.IsAny<ushort>(), It.IsAny<IDarkRiftSerializable>()))
					.Returns((ushort tag, IDarkRiftSerializable data) =>
						Message.Create<IDarkRiftSerializable>(tag, data)
					);

				var rpcBus = new RpcBus(factory.Object);

				var server = new TestEnv()
				{
					Name = name,
					EndPointMock = new Mock<IEndPoint>(),
					MessageFactory = factory,
					RpcBus = rpcBus,
					Sink = new RpcMessageSink<IEndPoint>(rpcBus)
				};
				return server;
			}

			public void Log(string message)
			{
				Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} [{Name}] {message}");
			}
		}

		private class PingRequest : IDarkRiftSerializable
		{
			public DateTime Now;
			public void Deserialize(DeserializeEvent e) => Now = new DateTime(e.Reader.ReadInt64());
			public void Serialize(SerializeEvent e) => e.Writer.Write(Now.Ticks);
			public override string ToString() => $"PingRequest {Now}";
		}

		private class PingResponse : IDarkRiftSerializable
		{
			public DateTime Now;
			public void Deserialize(DeserializeEvent e) => Now = new DateTime(e.Reader.ReadInt64());
			public void Serialize(SerializeEvent e) => e.Writer.Write(Now.Ticks);
			public override string ToString() => $"PingResponse {Now}";
		}

		public class MyCustomClient
		{
			private Dictionary<int, int> a;
			private IClient _client;
			private ClientEndPoint _clientEP;
			private RpcBus _rpcBus;
			private RpcMessageSink<IEndPoint> _sink;

			public MyCustomClient(IClient client)
			{
				var darkRiftServer = new DarkRiftServer(null);
				darkRiftServer.NetworkListenerManager.GetNetworkListeners();
				_client = client;
				_clientEP = new ClientEndPoint(_client);

				_client.MessageReceived += MessageHandler;
				_rpcBus = new RpcBus(null);
				_sink = new RpcMessageSink<IEndPoint>(_rpcBus);

				_sink.Subscribe<PingRequest, PingResponse>(PingRPC);
			}

			private PingResponse PingRPC(PingRequest arg)
			{
				Console.WriteLine(_client.ID);
				return new PingResponse();
			}

			private void MessageHandler(object sender, MessageReceivedEventArgs e)
			{
				using var message = e.GetMessage();
				_sink.HandleMessage(_clientEP, message);
			}
		}
	}
}
