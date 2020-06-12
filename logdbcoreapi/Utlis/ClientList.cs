using logdbcoreapi.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace logdbcoreapi.Utlis
{
    /// <summary>
    /// 传播类型。
    /// </summary>
    public enum SendType
    {
        /// <summary>
        /// 单向（一对一）
        /// </summary>
        Unicast,
        /// <summary>
        /// 广播（所有，群聊）
        /// </summary>
        Broadcast
    }
    /// <summary>
    /// 数据类型。
    /// </summary>
    public enum DataType
    {
        String,
        Json,
    }

    /// <summary>
    /// 维护连接用户。
    /// </summary>
    public class ClientList
    {
        public static List<WebSocket> clist = new List<WebSocket>();
        public static ConcurrentDictionary<string, List<WebSocket>> userdic = new ConcurrentDictionary<string, List<WebSocket>>();

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
        /// <summary>
        /// 每次新增时，先检测下这个这个聊天室中是否有异常的连接。
        /// 如果有，则移除异常的WebSocket。
        /// </summary>
        /// <param name="roomid"></param>
        private static void RemoveNotOpenState(string roomid)
        {
            if (userdic.Count > 0 && userdic.ContainsKey(roomid)) 
            {
                for(int i=0;i < userdic[roomid].Count;i++)
                {
                     List<WebSocket> sockets = userdic[roomid];
                    foreach (WebSocket socket in sockets)
                    if (socket.State != WebSocketState.Open)
                    {
                        userdic[roomid].Remove(socket);
                    }
                }
            }
        }
        public static void AddDicUser(string roomid, WebSocket socket)
        {
            if (userdic.Count <= 200)//两百个聊天室
            {
                RemoveNotOpenState(roomid);
                if (!userdic.ContainsKey(roomid))
                {
                    List<WebSocket> models = new List<WebSocket>();
                    models.Add(socket);
                    userdic.TryAdd(roomid, models);
                }
                else
                {
                    if (!userdic[roomid].Contains(socket))
                    {
                        userdic[roomid].Add(socket);//聊天室中添加人。
                    }
                }
            }
        }
        /// <summary>
        /// 移除聊天室中的某个成员。
        /// </summary>
        /// <param name="roomid"></param>
        /// <param name="smodel"></param>
        public static void RemoveDicUser(string roomid, WebSocket smodel)
        {
            if (userdic.ContainsKey(roomid))
            {
                userdic[roomid].Remove(smodel);
            }
        }
        /// <summary>
        /// 移除聊天室成员，及其销毁聊天室。
        /// </summary>
        public static void ClearDicAllUser(string roomid)
        {
            if(userdic.ContainsKey(roomid) && userdic[roomid].Count> 0)
            {
                userdic[roomid].Clear();
                userdic.Clear();
            }
        }
    }
}
