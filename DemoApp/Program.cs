using DemoApp;
using Microsoft.Extensions.Configuration;
using Nimiq.RPC.Models;
using Nimiq.RPC.Models.Steam;
using Nimiq.RPC.NimiqHttpClient;
using Nimiq.RPC.NimiqWebSocketClient;
using System.Text.Json;



class Program
{
    static async Task Main(string[] args)
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();


         // Get RPC settings from configuration
        var rpcSetting = configuration.GetSection("RPCSettings").Get<RPCSetting>();

        //var rpcSetting = new RPCSetting()
        //{
        //    RpcUrl = "ws://localhost:8648/ws",
        //    Username = "super",
        //    Password = "secret"
        //};


        //var client = new WebSocketClient(rpcSetting);


        //var requestBody = new RPCRequest("subscribeForHeadBlock", new object[] { false });

        //var subscription = await client.Subscribe<Block>(requestBody, CancellationToken.None);

        //await subscription.Next(
        //    response =>
        //    {
        //        switch (response)
        //        {
        //            case StreamResponse<Block>.Success success:
        //                Console.WriteLine($"Data: {success.Data?.Number}");
        //                break;
        //            case StreamResponse<Block>.Failure failure:
        //                Console.WriteLine($"Error: {failure.Error?.Message}");
        //                break;
        //        }
        //        return Task.CompletedTask;
        //    },
        //    CancellationToken.None
        //);

        //await subscription.Close();

        var rpcSetting = new RPCSetting()
        {
            RpcUrl = "http://validator-test.nimiqpocket.com:8648",
            Username = "super",
            Password = "secret"
        };

        var httpClient = new NimiqHttpClient(rpcSetting);
        var httpResponse = httpClient.GetByMethod<JsonElement>("getEpochNumber", new object[] {  });
        Console.WriteLine($"Data: {httpResponse.Result}");
    }
}
