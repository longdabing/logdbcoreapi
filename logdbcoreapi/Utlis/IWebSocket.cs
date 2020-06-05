using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace logdbcoreapi.Utlis
{
    public interface IWebSocket
    {
        Task CreateSocket(HttpContext httpContext);
        Task<string> ReceiveDataAsync(WebSocket webSocket, CancellationToken cancellationToken);
        Task SendDataAsync(string msg, WebSocket webSocket);
    }
}
