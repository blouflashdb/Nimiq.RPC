namespace Nimiq.RPC;

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

public class Request
{
    public required string Method { get; set; }
    public required object[] Params { get; set; }
}

internal class RequestBody
{
    [JsonPropertyName("method")]
    public required string Method { get; set; }

    [JsonPropertyName("params")]
    public required object[] Params { get; set; }

    [JsonPropertyName("id")]
    public required int Id { get; set; }

    [JsonPropertyName("jsonrpc")]
    public required string Jsonrpc { get; set; }
}

public abstract class StreamResponse<T>
{
    public sealed class Success(T? data) : StreamResponse<T>
    {
        public T? Data { get; } = data;
    }

    public sealed class Failure(ErrorStreamReturn? error) : StreamResponse<T>
    {
        public ErrorStreamReturn? Error { get; } = error;
    }
}

public class ErrorStreamReturn
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

public class WebSocketClient(Uri url)
{
    private int id = 0;

    public async Task<ISubscription<T>> Subscribe<T>(
        Request request,
        CancellationToken cancellationToken
    )
    {
        var ws = new ClientWebSocket();
        await ws.ConnectAsync(url, cancellationToken);
        var requestBody = new RequestBody
        {
            Method = request.Method,
            Params = request.Params,
            Jsonrpc = "2.0",
            Id = id++,
        };
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
