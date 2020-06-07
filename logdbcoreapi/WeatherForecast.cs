using System;
using System.Collections.Generic;
using System.Net.WebSockets;

namespace logdbcoreapi
{
    public enum SendType
    {
        SystemMsg,
        Unicast,
        Broadcast
    }
    public enum DataType
    {
        String,
        Json,
    }
    public class WebSocketModel
    {
        public WebSocket Sk { get; set; }
        public Guid Id { get; set; }
    }
    public class ChatRoom
    {
        public List<WebSocketModel> UserList { get; set; }
        public Guid RoomId { get; set; }
    }
    public class MessageModel
    {
        public DataType DataType { get; set; }
        public SendType SendType { get; set; }
        public object Data { get; set; }
        public string SenderName { get; set; }
        public Guid SenderId { get; set; }
        public Guid TargetId { get; set; }
    }
    public static class WebSocketHelper
    {
        /// <summary>
        /// websocket容器，可以当作缓存使用
        /// </summary>
        public static ChatRoom Root = new ChatRoom
        {
            UserList = new List<WebSocketModel>(),
            RoomId = Guid.NewGuid(),
        };

    }
}
