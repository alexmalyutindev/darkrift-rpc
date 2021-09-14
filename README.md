Simple DarkRift RPC add-on
---

Examples
---
- Register RPCs:
```c#
RpcRegister.Register<MyRequest>(0);
RpcRegister.Register<MyResponse>(1);
```
- Subscribe:
```c#
var messageFactory = new MyMessageFactory();
var scheduler = new RpcScheduler(messageFactory);
var processor = new RpcProcessor(scheduler);

processor.Subscribe((MyRequest r, IEndPoint ep) =>
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

var response = await scheduler.Call<MyRequest, MyResponse>(client, request);
```
