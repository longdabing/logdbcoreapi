using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace logdbcoreapi.Utlis
{
    /// <summary>
    /// 维护连接用户。
    /// </summary>
    public class ClientList
    {
        public static List<WebSocket> clist = new List<WebSocket>();

        public static void AddUser(WebSocket socket)
        {
            if (clist.Count <= 500)
            {
                if (!clist.Contains(socket))
                {
                    clist.Add(socket);
                }
            }
        }
        public static void RemoveUser(WebSocket socket)
        {
            clist.Remove(socket);
        }
        /// <summary>
        /// 移除所有用户
        /// </summary>
        public static void ClearAllUser()
        {
            if (clist.Count > 0)
            {
                clist.Clear();
            }
        }
    }
}
