
using Nimiq.RPC.Models;
using Nimiq.RPC.NimiqWebSocketClient;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;


namespace Nimiq.RPC.NimiqWebSocketClient
{


    public class WebSocketClient
    {
        private readonly ClientWebSocket _ws = new ClientWebSocket();
        private readonly Uri _url;
        private readonly string _username;
        private readonly string _password;

        public WebSocketClient(RPCSetting setting)
        {
            _url = new Uri(setting.RpcUrl);
            _username = setting.Username;
            _password = setting.Password;
        }
    
        public async Task<ISubscription<T>> Subscribe<T>(
            RPCRequest request,
            CancellationToken cancellationToken
        )
        {

            // Add basic authentication if username and password are provided
            if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
            {
                var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_password}"));
                _ws.Options.SetRequestHeader("Authorization", "Basic " + auth);
            }

            // Connect to the WebSocket server
            await _ws.ConnectAsync(_url, cancellationToken);

            // Create a new RPCRequest object
            var requestBody = new RPCRequest(request.Method, request.Params);

            // Create a new Subscription object
            var args = new Subscription<T>(_ws);

            // Send the RPCRequest object to the server
            await _ws.SendAsync(
                new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(requestBody))),
                WebSocketMessageType.Text,
                true,
                cancellationToken
            );
            return args;
        }


    }


}