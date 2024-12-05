using Nimiq.RPC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nimiq.RPC
{
    public interface INimiqHttpClient
    {
        Task<T> GetByMethod<T>(string method, object[] @params = null);

    }

    public class NimiqHttpClient : INimiqHttpClient
    {

        private readonly HttpClient _httpClient;
        private readonly string _rpcUrl;


        public NimiqHttpClient(HttpClient httpClient, RPCSetting rpcSettings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _rpcUrl = rpcSettings.RpcUrl; 
            var byteArray = Encoding.ASCII.GetBytes($"{rpcSettings.Username}:{rpcSettings.Password}");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }


        public async Task<T> GetByMethod<T>(string method, object[]? @params = null)
        {
            var rpcRequest = new RPCRequest(method, @params ?? Array.Empty<object>());

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Serialize the RpcRequest to JSON
            var requestContent = new StringContent(JsonSerializer.Serialize(rpcRequest, options), Encoding.UTF8, "application/json");

            // Send the HTTP POST request
            var httpResponse = await _httpClient.PostAsync(_rpcUrl, requestContent);

            // Ensure the response is successful
            httpResponse.EnsureSuccessStatusCode();

            // Parse and return the result from the JSON response
            var responseString = await httpResponse.Content.ReadAsStringAsync();
            var responseJson = JsonSerializer.Deserialize<JsonElement>(responseString);

            if (responseJson.TryGetProperty("result", out var result))
            {
                return JsonSerializer.Deserialize<T>(result.GetRawText(), options) ?? throw new Exception("Deserialization returned null.");
            }
            else if (responseJson.TryGetProperty("error", out var error))
            {
                throw new Exception($"RPC Error: {error}");
            }
            else
            {
                throw new Exception("Invalid RPC response format.");
            }
        }
    }
}
