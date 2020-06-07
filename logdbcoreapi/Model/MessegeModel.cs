using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace logdbcoreapi.Model
{
    /// <summary>
    /// 传播类型
    /// </summary>
    public enum SendType
    {
        Unicast,
        Broadcast
    }

    /// <summary>
    /// 数据类型
    /// </summary>
    public enum DataType
    {
        String,
        Json,
    }
    public class MessegeModel
    {
        public string RoomId { get; set; }
        public DataType DataType { get; set; }
        public SendType SendType { get; set; }
        public object Data { get; set; }
        public string SenderName { get; set; }
        public Guid SenderId { get; set; }
        public Guid TargetId { get; set; }
    }
}
