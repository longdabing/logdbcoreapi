using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace logdbcoreapi.Utlis
{
    public class WebSocketChat: IWebSocket
    {

        private readonly ILogger<WebSocketChat> _logger;

        //private readonly WebSocket _socket;
        //WebSocketChat(WebSocket socket)
        //{
        //    this._socket = socket;
        //}

        /// <summary>
        /// 创建链接
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task CreateSocket(HttpContext httpContext)
        {

            //if (!httpContext.WebSockets.IsWebSocketRequest)
            //    return;
            var socket = await httpContext.WebSockets.AcceptWebSocketAsync();

            var result = await ReceiveDataAsync(socket, CancellationToken.None);

        }

        /// <summary>
        /// 接收客户端数据
        /// </summary>
        /// <param name="webSocket">webSocket 对象</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public  async Task<string> ReceiveDataAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            string oldmsg = "";
            WebSocketReceiveResult result;

            do
            {
                var ms = new MemoryStream();
                var buffer = new ArraySegment<byte>(new byte[1024 * 2]);
                result = await webSocket.ReceiveAsync(buffer, cancellationToken);
                if ("Close".Equals(result.MessageType.ToString()))
                {
                    break;
                }
                ms.Write(buffer.Array, buffer.Offset, result.Count - buffer.Offset);
                ms.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(ms);
                var message = reader.ReadToEnd();
                reader.Dispose();
                ms.Dispose();
                if (!string.IsNullOrEmpty(message))
                {

                    await SendDataAsync(message, webSocket);//把接收到的信息发出去
                }
                oldmsg = message;

            } while (result.EndOfMessage);

            return "";
        }
        /// <summary>
        /// 向客户端发送数据 
        /// </summary>
        /// <param name="msg">数据</param>
        /// <param name="webSocket">socket对象  sleep 心跳周期</param>
        /// <returns></returns>
        public  async Task SendDataAsync(string msg, WebSocket webSocket)
        {
            try
            {
                CancellationToken cancellation = default(CancellationToken);
                var buf = Encoding.UTF8.GetBytes(msg);
                var segment = new ArraySegment<byte>(buf);
                await webSocket.SendAsync(segment, WebSocketMessageType.Text, true, cancellation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        /// 路由绑定处理
        /// </summary>
        /// <param name="app"></param>
        public static void Map(IApplicationBuilder app)
        {
            //app.UseWebSockets();
            //app.Use(CreateSocket(HttpContext cs));
        }
    }
}
