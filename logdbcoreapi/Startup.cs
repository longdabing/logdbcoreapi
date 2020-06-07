using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using logdbcoreapi.Utlis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace logdbcoreapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IWebSocket, WebSocketChat>();//注入服务

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseDefaultFiles().UseStaticFiles();//使用静态文件

            app.UseRouting();
            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = new TimeSpan(0, 0, 15)
            }); //使用websoket 2020.06.04
            #region //测试
            //app.Use(async (context, next) =>
            //{
            //    //if (context.Request.Path == "/ws")
            //    {
            //        if (context.WebSockets.IsWebSocketRequest)
            //        {
            //            #region 创建socket连接                      
            //            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            //            var msg = JsonConvert.SerializeObject(new MessageModel
            //            {
            //                DataType = DataType.Json,
            //                SendType = SendType.SystemMsg,
            //                SenderName = "Server",
            //                Data = new { Id = Guid.NewGuid(), UserName = "NolenJ" }
            //            });
            //            byte[] byteArray = System.Text.Encoding.Default.GetBytes(msg);
            //            //连接成功，发送反馈，这里用了自定义的数据模板，前端根据消息类型做处理。
            //            //WebSocket创建连接在浏览器network可以看到请求，但是不像ajax请求，给不了返回值。
            //            await webSocket.SendAsync(new ArraySegment<byte>(byteArray), WebSocketMessageType.Text, true, CancellationToken.None);
            //            //保存到socket容器
            //            WebSocketHelper.Root.UserList.Add(new WebSocketModel
            //            {
            //                Id = Guid.NewGuid(),
            //                Sk = webSocket
            //            });
            //            #endregion
            //            //连接成功后，丢给自定义收发数据的管理工具
            //            await Echo(webSocket);
            //        }
            //        else
            //        {
            //            context.Response.StatusCode = 400;

            //        }
            //    }
            //    //else
            //    //{
            //    //    await next();
            //    //}

            //});
            #endregion
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        #region WebSockets相关
        private async Task Echo(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!webSocket.CloseStatus.HasValue)
            {
                //拆包
                var msg = JsonConvert.DeserializeObject<MessageModel>(Encoding.Default.GetString(buffer));
                switch (msg.SendType)
                {
                    case SendType.Broadcast:
                        List<Task> tasks = new List<Task>();
                        WebSocketHelper.Root.UserList.ForEach(c =>
                        {
                            tasks.Add(c.Sk.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None));
                        });
                        Task.WaitAll(tasks.ToArray());
                        break;
                    case SendType.Unicast:
                        Task.WaitAll(new List<Task>() {
                            WebSocketHelper.Root.UserList.Where(c => c.Id == msg.TargetId).FirstOrDefault().Sk.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None),
                            //给自己发消息表示我的消息已送达
                            webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None)
                        }.ToArray());
                        break;
                }
                //重置消息容器
                buffer = new byte[1024 * 4];
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            WebSocketHelper.Root.UserList.Remove(WebSocketHelper.Root.UserList.Where(c => c.Sk == webSocket).FirstOrDefault());
            await webSocket.CloseAsync(webSocket.CloseStatus.Value, webSocket.CloseStatusDescription, CancellationToken.None);
        }
        #endregion
    }
}
