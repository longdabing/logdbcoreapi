using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    public class MessageModel
    {
        [Key]
        public int Id { get; set; }
        public string RoomId { get; set; }
        [NotMapped]
        public DataType DataType { get; set; }
        [NotMapped]
        public SendType SendType { get; set; }
        public string Data { get; set; }
        public string SenderName { get; set; }
        public string SenderId { get; set; }
        public string TargetId { get; set; }
    }
}
