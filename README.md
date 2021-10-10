Simple DarkRift RPC add-on
---

Work in progress. API can be changed. For more information, see [Board](https://github.com/alexmalyutindev/darkrift-rpc/projects/1). <br>
You are welcome to contribute your changes/suggestions via Pull Request or [Issues](https://github.com/alexmalyutindev/darkrift-rpc/issues/new).

Examples
---
- Register RPCs:
```c#
RpcRegistry.RegisterRequest<MyRequest>(0);
RpcRegistry.RegisterResponse<MyResponse>(1);
```
- Subscribe:
```c#
var messageFactory = new MyMessageFactory();
var rpcBus = new RpcBus<IEndPoint>(messageFactory); // You may use you custom type ensted of 'IEndPoint'
var sink = new RpcMessageSink(rpcBus);

sink.Subscribe((IEndPoint sender, MyRequest request) =>
{
    return new MyRequest();
});
```
- Call RPC:
```c#
var client = new ClientEndPoint(...); // for Server->Client rpc
var server = new ServerEndPoint(...); // for Client->Server rpc

var response = await rpcBus.Call<MyRequest, MyResponse>(client, request);
```
- On DarkRift server side:
```c#
private void OnClientConnected(object sender, ClientConnectedEventArgs e)
{
    var clientData = new MyClientData(Client);
    e.Client.MessageReceived += (sender, e) =>
    {
        using var message = e.GetMessage();
        _rpcSink.HandleMessage(clientData, message);
    };
}
```
