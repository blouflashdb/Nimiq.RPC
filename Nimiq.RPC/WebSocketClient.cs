namespace Nimiq.RPC;

using Nimiq.RPC.Models;
using Nimiq.RPC.Models.Steam;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

public interface ISubscription<T>
{
    Task Next(Func<StreamResponse<T>, Task> callback, CancellationToken cancellationToken);
    Task Close();
}



public class WebSocketClient(Uri url)
{

    public async Task<ISubscription<T>> Subscribe<T>(
        RPCRequest request,
        CancellationToken cancellationToken
    )
    {
        var ws = new ClientWebSocket();
        await ws.ConnectAsync(url, cancellationToken);
        var requestBody = new RPCRequest(request.Method, request.Params);
   
        var args = new Subscription<T>(ws);
        await ws.SendAsync(
            new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(requestBody))),
            WebSocketMessageType.Text,
            true,
            cancellationToken
        );
        return args;
    }
}

public class Subscription<T>(ClientWebSocket ws) : ISubscription<T>
{
    public async Task Close()
    {
        if (ws.State == WebSocketState.Open)
        {
            await ws.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Closing",
                CancellationToken.None
            );
        }
    }

    public async Task Next(
        Func<StreamResponse<T>, Task> callback,
        CancellationToken cancellationToken
    )
    {
        var buffer = new byte[1024 * 4];
        var encoding = Encoding.UTF8;
        try
        {
            while (ws.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var result = await ws.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    cancellationToken
                );
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await ws.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Closing",
                        CancellationToken.None
                    );
                    break;
                }
                var message = encoding.GetString(buffer, 0, result.Count);
                var payload = JsonSerializer.Deserialize<JsonElement>(message);
                if (payload.TryGetProperty("error", out var errorElement))
                {
                    var error = JsonSerializer.Deserialize<ErrorStreamReturn>(
                        errorElement.GetRawText()
                    );
                    await callback(new StreamResponse<T>.Failure(error));
                    continue;
                }
                if (payload.TryGetProperty("params", out var paramsElement))
                {
                    var resultData = paramsElement.GetProperty("result").GetProperty("data");
                    T? data = JsonSerializer.Deserialize<T>(resultData.GetRawText());
                    await callback(new StreamResponse<T>.Success(data));
                }
            }
        }
        catch (Exception ex)
        {
            await callback(
                new StreamResponse<T>.Failure(
                    new ErrorStreamReturn { Code = -1, Message = ex.Message }
                )
            );
        }
    }
}
