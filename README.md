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
var rpcBus = new RpcBus(messageFactory);
var sink = new RpcMessageSink(rpcBus);

sink.Subscribe((MyRequest r, IEndPoint ep) =>
{
    return new MyRequest();
});
```
- Call RPC:
```c#
var client = new ClientEndPoint(client); // for Server->Client rpc
var server = new ServerEndPoint(client); // for Client->Server rpc

var response = await rpcBus.Call<MyRequest, MyResponse>(client, request);
```
