Simple DarkRift RPC add-on
---

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
var scheduler = new RpcScheduler(messageFactory);
var sink = new RpcMessageSink(scheduler);

sink.Subscribe((MyRequest r, IEndPoint ep) =>
{
    return new MyRequest();
});
```
- Call RPC:
```c#
var client = new ClientEndPoint(client); // for Server->Client rpc
var server = new ServerEndPoint(client); // for Client->Server rpc

var messageFactory = new MyMessageFactory();
var scheduler = new RpcScheduler(messageFactory);
var sink = new RpcMessageSink(scheduler);

var response = await scheduler.Call<MyRequest, MyResponse>(client, request);
```
