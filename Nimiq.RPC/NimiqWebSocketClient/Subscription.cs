using Nimiq.RPC.Models.Steam;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Nimiq.RPC.NimiqWebSocketClient
{
    public interface ISubscription<T>
    {
        Task Next(Func<StreamResponse<T>, Task> callback, CancellationToken cancellationToken);
        Task Close();
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

}
