using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nimiq.RPC.Models
{
    public class RPCRequest
    {
        [JsonPropertyName("jsonrpc")]
        public string Jsonrpc { get; set; } = "2.0";

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("params")]
        public object[] Params { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        public RPCRequest(string method, object[] @params = null, int id = 1)
        {
            Jsonrpc = "2.0";
            Method = method;
            Params = @params ?? new object[] { }; // Default to an empty array if no parameters are provided.
            Id = id;
        }
    }
}
