using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimiq.RPC.Models.Steam
{
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

}
