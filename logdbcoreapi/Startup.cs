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
            services.AddSingleton<IWebSocket, WebSocketChat>();//ע�����

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseDefaultFiles().UseStaticFiles();//ʹ�þ�̬�ļ�

            app.UseRouting();
            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = new TimeSpan(0, 0, 15)
            }); //ʹ��websoket 2020.06.04
            #region //����
            //app.Use(async (context, next) =>
            //{
            //    //if (context.Request.Path == "/ws")
            //    {
            //        if (context.WebSockets.IsWebSocketRequest)
            //        {
            //            #region ����socket����                      
            //            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            //            var msg = JsonConvert.SerializeObject(new MessageModel
            //            {
            //                DataType = DataType.Json,
            //                SendType = SendType.SystemMsg,
            //                SenderName = "Server",
            //                Data = new { Id = Guid.NewGuid(), UserName = "NolenJ" }
            //            });
            //            byte[] byteArray = System.Text.Encoding.Default.GetBytes(msg);
            //            //���ӳɹ������ͷ��������������Զ��������ģ�壬ǰ�˸�����Ϣ����������
            //            //WebSocket���������������network���Կ������󣬵��ǲ���ajax���󣬸����˷���ֵ��
            //            await webSocket.SendAsync(new ArraySegment<byte>(byteArray), WebSocketMessageType.Text, true, CancellationToken.None);
            //            //���浽socket����
            //            WebSocketHelper.Root.UserList.Add(new WebSocketModel
            //            {
            //                Id = Guid.NewGuid(),
            //                Sk = webSocket
            //            });
            //            #endregion
            //            //���ӳɹ��󣬶����Զ����շ����ݵĹ�����
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
        #region WebSockets���
        private async Task Echo(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!webSocket.CloseStatus.HasValue)
            {
                //���
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
                            //���Լ�����Ϣ��ʾ�ҵ���Ϣ���ʹ�
                            webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None)
                        }.ToArray());
                        break;
                }
                //������Ϣ����
                buffer = new byte[1024 * 4];
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            WebSocketHelper.Root.UserList.Remove(WebSocketHelper.Root.UserList.Where(c => c.Sk == webSocket).FirstOrDefault());
            await webSocket.CloseAsync(webSocket.CloseStatus.Value, webSocket.CloseStatusDescription, CancellationToken.None);
        }
        #endregion
    }
}
