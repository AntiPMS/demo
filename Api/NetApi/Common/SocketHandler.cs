using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NetApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetApi.Common
{
    /// <summary>
    /// websocket中间件实现类
    /// </summary>
    public class MyWebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebsocketManager _wsManage;

        public MyWebSocketMiddleware(IWebsocketManager wsManage, RequestDelegate next)
        {
            _next = next;
            _wsManage = wsManage;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    try
                    {
                        if (context.Request.QueryString.HasValue)
                        {
                            var client = _wsManage.Add(context, webSocket);//加入到组里
                            await Handle(client);
                        }
                        else
                        {
                            context.Response.StatusCode = 400;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("报错：{0}", ex.Message);
                        await context.Response.WriteAsync("closed");
                    }
                }
                else
                {
                    context.Response.StatusCode = 404;
                }
            }
            else
            {
                // Call the next delegate/middleware in the pipeline
                await _next(context);
            }
        }

        /// <summary>
        /// 消息处理方法 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task Handle(WebSocketClient client)
        {
            WebSocketReceiveResult result = null;
            do
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4096]); ;//接收ws消息
                bool isSendSuccess = true;//是否发送成功
                string webSocketData = "";

                #region 分包读取消息
                do
                {
                    result = await client.socket.ReceiveAsync(buffer, CancellationToken.None);
                    var messageBytes = buffer.Skip(buffer.Offset).Take(result.Count).ToArray();
                    webSocketData += Encoding.UTF8.GetString(messageBytes);
                }
                while (!result.EndOfMessage);
                #endregion

                if (result.MessageType == WebSocketMessageType.Text && !result.CloseStatus.HasValue)
                {
                    var messageModel = JsonConvert.DeserializeObject<WebSocketClientModel>(webSocketData); //json序列化
                    _wsManage.SendMsg(messageModel, out isSendSuccess); //消息交给路由发送
                    if (isSendSuccess)
                    {
                        //消息已读。
                    }
                }
            }
            while (!result.CloseStatus.HasValue);
            //Console.WriteLine($"ws客户端:{client.Id}-{client.Name}断开[{client.TargetId}]连接.");
            _wsManage.Remove(client);
        }

    }

    /// <summary>
    /// websocket管理接口
    /// </summary>
    public interface IWebsocketManager
    {
        /// <summary>
        /// 添加新websocket
        /// </summary>
        /// <param name="context"></param>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        public WebSocketClient Add(HttpContext context, WebSocket webSocket);

        /// <summary>
        /// 移除websocket
        /// </summary>
        /// <param name="client"></param>
        public void Remove(WebSocketClient client);

        /// <summary>
        /// 根据发送人Id查找websocket实例
        /// </summary>
        /// <param name="senderId"></param>
        /// <returns></returns>
        public WebSocketClient GetBySenderId(string senderId);

        /// <summary>
        /// 根据目标Id查找除发送人之外的所有websocket实例
        /// </summary>
        /// <param name="senderId">发送人Id</param>
        /// <param name="targetId">目标Id</param>
        /// <returns></returns>
        public IEnumerable<WebSocketClient> GetByTargetId(string senderId, string targetId);

        /// <summary>
        /// 发送消息给客户端
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isSuccess"></param>
        public void SendMsg(WebSocketClientModel model, out bool isSuccess);

        /// <summary>
        /// 消息类型枚举
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetMsgTypeEnum();
    }

    /// <summary>
    /// websocket管理类
    /// </summary>
    public class WebsocketManager : IWebsocketManager
    {
        private IConfiguration _conf { get; }

        public WebsocketManager(IConfiguration configuration)
        {
            _conf = configuration;
        }

        private static List<WebSocketClient> _clients = new List<WebSocketClient>();

        /// <summary>
        /// 添加新websocket
        /// </summary>
        /// <param name="context"></param>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        public WebSocketClient Add(HttpContext context, WebSocket webSocket)
        {
            var queryStr = context.Request.QueryString.Value.Split("&");
            var client = new WebSocketClient
            {
                Id = queryStr[0].Substring(queryStr[0].IndexOf('=') + 1),
                TargetId = queryStr[1].Substring(queryStr[1].IndexOf('=') + 1),
                socket = webSocket
            };
            if (!_clients.Where(m => m.Id == client.Id && m.TargetId == client.TargetId).Any())
            {
                _clients.Add(client);
            }
            SendHisMessage(client);//发送历史消息
            return client;
        }

        /// <summary>
        /// 移除websocket
        /// </summary>
        /// <param name="client"></param>
        public void Remove(WebSocketClient client) => _clients.Remove(client);

        /// <summary>
        /// 根据发送人Id查找websocket实例
        /// </summary>
        /// <param name="senderId"></param>
        /// <returns></returns>
        public WebSocketClient GetBySenderId(string senderId) => _clients.FirstOrDefault(c => c.Id == senderId);

        /// <summary>
        /// 根据目标Id查找除发送人之外的所有websocket实例
        /// </summary>
        /// <param name="senderId">发送人Id</param>
        /// <param name="targetId">目标Id</param>
        /// <returns></returns>
        public IEnumerable<WebSocketClient> GetByTargetId(string senderId, string targetId) => _clients.Where(m => m.TargetId == targetId && m.Id != senderId);

        /// <summary>
        /// 发送消息给客户端
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isSuccess"></param>
        public void SendMsg(WebSocketClientModel model, out bool isSuccess) => MessageRoute(model, out isSuccess);

        /// <summary>
        /// 消息类型枚举
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetMsgTypeEnum()
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            Enum.GetNames(typeof(MsgType))
                .ToList()
                .ForEach(m => result.Add(m, Convert.ToInt32(Enum.Parse(typeof(MsgType), m)))
            );
            return result;
        }

        private void SendHisMessage(WebSocketClient client)
        {
            using (var db = new NetApiContextForMsg(_conf))
            {
                var msgTypeList = new List<sbyte>() { (sbyte)MsgType.Text, (sbyte)MsgType.Img };
                //默认先取 日期倒排 历史近20条
                var hisMsg = db.UserMsg
                               .Where(m => m.TargetId == client.TargetId && msgTypeList.Contains((sbyte)m.MsgType))
                               .OrderByDescending(m => m.SendDate)
                               .Take(20)
                               .OrderBy(m => m.SendDate);
                if (hisMsg.Any())
                {
                    hisMsg.ToList().ForEach(m =>
                    {
                        WebSocketClientModel message = new WebSocketClientModel
                        {
                            SenderId = m.SenderId,
                            SenderName = m.SenderId,
                            MsgType = (MsgType)m.MsgType,
                            Msg = m.Msg,
                            SendDate = m.SendDate
                        };
                        client.SendMsg(JsonConvert.SerializeObject(message));
                    });
                }
            }
        }

        private void Save2DataBase(string senderId, string targetId, MsgType msgType, string msg)
        {
            using (var db = new NetApiContextForMsg(_conf))
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.UserMsg.Add(new UserMsg
                        {
                            Id = Guid.NewGuid().ToString(),
                            SenderId = senderId,
                            TargetId = targetId,
                            MsgType = (sbyte)msgType,
                            Msg = msg,
                        });
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                    }
                }
            }
        }

        private void MessageRoute(WebSocketClientModel message, out bool isSuccess)
        {
            isSuccess = true;
            try
            {
                switch (message.MsgType)
                {
                    //case MsgType.Join:
                    //    client.SendMsg(logJoin);
                    //    break;
                    case MsgType.Text:
                    case MsgType.Img:
                        var client = GetBySenderId(message.SenderId);
                        if (client != null && !string.IsNullOrEmpty(client.TargetId))
                        {
                            Save2DataBase(client.Id, client.TargetId, message.MsgType, message.Msg);

                            var clients = GetByTargetId(client.Id, client.TargetId);
                            if (clients.Any())
                            {
                                //群组广播发送
                                var clientList = clients.ToList();
                                clientList.ForEach(c =>
                                {
                                    c.SendMsg<string>(JsonConvert.SerializeObject(message));
                                });
                            }
                            else
                            {
                                isSuccess = false;
                            }
                        }
                        else
                        {
                            isSuccess = false;
                        }
                        break;
                    case MsgType.System:
                        Save2DataBase(message.SenderId, message.TargetId, message.MsgType, message.Msg);

                        var clientSys = GetByTargetId(message.SenderId, message.TargetId);
                        if (clientSys.Any())
                        {
                            //群组广播发送
                            var clientList = clientSys.ToList();
                            clientList.ForEach(c =>
                            {
                                c.SendMsg<string>(JsonConvert.SerializeObject(message));
                            });
                        }
                        break;
                    case MsgType.Exit:
                        var extClient = GetBySenderId(message.SenderId);
                        extClient.SendMsg<string>(JsonConvert.SerializeObject(new WebSocketClientModel { }));
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                isSuccess = false;
            }
        }
    }

    #region 模型及枚举类
    public enum MsgType
    {
        /// <summary>
        /// 加入
        /// </summary>
        Join = 0,

        /// <summary>
        /// 纯文本消息
        /// </summary>
        Text = 1,

        /// <summary>
        /// 图片
        /// </summary>
        Img = 2,

        /// <summary>
        /// 系统发送的消息
        /// </summary>
        System = 7,

        /// <summary>
        /// 设置已读
        /// </summary>
        SetReadStatus = 8,

        /// <summary>
        /// 退出
        /// </summary>
        Exit = 9
    }

    /// <summary>
    /// websocket客户端模型
    /// </summary>
    public class WebSocketClient
    {
        /// <summary>
        /// 客户端主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 客户端名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 客户端群组主键
        /// </summary>
        public string TargetId { get; set; }

        /// <summary>
        /// websocket实例
        /// </summary>
        public WebSocket socket { get; set; }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendMsg<T>(T msg)
        {
            if (typeof(T).Equals(typeof(string)))
            {
                if (socket != null && socket.State == WebSocketState.Open)
                {
                    var msgs = Encoding.UTF8.GetBytes(msg as string);
                    socket.SendAsync(new ArraySegment<byte>(msgs), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }

    /// <summary>
    /// websocket客户端 请求、相应消息模型
    /// </summary>
    public class WebSocketClientModel
    {
        /// <summary>
        /// 客户端主键
        /// </summary>
        public string SenderId { get; set; }

        /// <summary>
        /// 客户端名称
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// 客户端隶属群组主键
        /// </summary>
        public string TargetId { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public MsgType MsgType { get; set; }

        /// <summary>
        /// 具体消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 发送日期
        /// </summary>
        public DateTime? SendDate { get; set; }
    }

    #endregion

}
