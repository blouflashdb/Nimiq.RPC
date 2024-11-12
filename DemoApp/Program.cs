using DemoApp;
using Nimiq.RPC;

var client = new WebSocketClient(new Uri("ws://localhost:8648/ws"));

var requestBody = new Request { Method = "subscribeForHeadBlock", Params = [false] };

var subscription = await client.Subscribe<Block>(requestBody, CancellationToken.None);

await subscription.Next(
    response =>
    {
        switch (response)
        {
            case StreamResponse<Block>.Success success:
                Console.WriteLine($"Data: {success.Data?.Number}");
                break;
            case StreamResponse<Block>.Failure failure:
                Console.WriteLine($"Error: {failure.Error?.Message}");
                break;
        }
        return Task.CompletedTask;
    },
    CancellationToken.None
);

await subscription.Close();
