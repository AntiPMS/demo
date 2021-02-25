using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NetApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
                            _wsManage.Add(context, webSocket);//加入到组里
                            await Handle(webSocket);
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
        private async Task Handle(WebSocket client)
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
                    result = await client.ReceiveAsync(buffer, CancellationToken.None);
                    var messageBytes = buffer.Skip(buffer.Offset).Take(result.Count).ToArray();
                    webSocketData += Encoding.UTF8.GetString(messageBytes);
                }
                while (!result.EndOfMessage);
                #endregion

                if (result.MessageType == WebSocketMessageType.Text && !result.CloseStatus.HasValue)
                {
                    var messageModel = JsonConvert.DeserializeObject<WebSocketClientModel>(webSocketData); //json序列化
                    messageModel.SendDate = DateTime.Now;
                    _wsManage.SendMsg(messageModel, client, out isSendSuccess); //消息交给路由发送
                    if (isSendSuccess)
                    {
                        //消息已读。
                    }
                }
            }
            while (!result.CloseStatus.HasValue);
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
        public void Add(HttpContext context, WebSocket webSocket);

        /// <summary>
        /// 移除websocket
        /// </summary>
        /// <param name="client"></param>
        public void Remove(WebSocketClient client);

        /// <summary>
        /// 根据发送人Id及目标Id查找websocket实例
        /// </summary>
        /// <param name="senderId"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public WebSocketClient GetSender(string senderId, string targetId);

        /// <summary>
        /// 根据目标Id查找websocket实例
        /// </summary>
        /// <param name="targetId">目标Id</param>
        /// <returns></returns>
        public IEnumerable<WebSocketClient> GetByTargetId(string targetId);

        /// <summary>
        /// 获取当前所有在线socket
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WebSocketClient> GetCurrentAll();

        /// <summary>
        /// 发送消息给所有客户端
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isSuccess"></param>
        public void SendMsg(WebSocketClientModel model, out bool isSuccess);

        /// <summary>
        /// 发送消息给客户端(除自己)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentClient"></param>
        /// <param name="isSuccess"></param>
        public void SendMsg(WebSocketClientModel model, WebSocket currentClient, out bool isSuccess);

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
        public void Add(HttpContext context, WebSocket webSocket)
        {
            WebSocketClient client = new WebSocketClient();
            var queryStr = context.Request.QueryString.Value.Split("&");
            var id = queryStr[0].Substring(queryStr[0].IndexOf('=') + 1);
            var targetId = queryStr[1].Substring(queryStr[1].IndexOf('=') + 1);
            var oldClients = _clients.Where(m => m.Id == id && m.TargetId == targetId);

            //如果当前客户端从未连接过，则新增一个实例
            if (!oldClients.Any())
            {
                var newClient = new WebSocketClient
                {
                    Id = id,
                    TargetId = targetId,
                    socketList = new List<WebSocket>() { webSocket }
                };
                _clients.Add(newClient);
                client = newClient;
            }
            else//若连接过，则把socket实例添加进来，并移除已断开的客户端
            {
                var oldClient = oldClients.FirstOrDefault();
                List<WebSocket> newSocketList = new List<WebSocket>() { webSocket };

                oldClient.socketList.ForEach(m =>
                {
                    if (!m.CloseStatus.HasValue)
                    {
                        newSocketList.Add(m);
                    }
                });

                oldClient.socketList = newSocketList;
                client = oldClient;
            }
            //SendHisMessage(client, webSocket);//发送历史消息
        }

        /// <summary>
        /// 移除websocket
        /// </summary>
        /// <param name="client"></param>
        public void Remove(WebSocketClient client) => _clients.Remove(client);

        /// <summary>
        /// 根据发送人Id及目标Id查找websocket实例
        /// </summary>
        /// <param name="senderId"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public WebSocketClient GetSender(string senderId, string targetId) => _clients.FirstOrDefault(c => c.Id == senderId && c.TargetId == targetId);

        /// <summary>
        /// 根据目标Id查找websocket实例
        /// </summary>
        /// <param name="targetId">目标Id</param>
        /// <returns></returns>
        public IEnumerable<WebSocketClient> GetByTargetId(string targetId) => _clients.Where(m => m.TargetId == targetId);

        /// <summary>
        /// 获取当前所有在线socket
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WebSocketClient> GetCurrentAll() => _clients.ToList();

        /// <summary>
        /// 发送消息给所有客户端
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isSuccess"></param>
        public void SendMsg(WebSocketClientModel model, out bool isSuccess) => MessageRoute(model, null, out isSuccess);

        /// <summary>
        /// 发送消息给客户端(除自己)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentClient"></param>
        /// <param name="isSuccess"></param>
        public void SendMsg(WebSocketClientModel model, WebSocket currentClient, out bool isSuccess) => MessageRoute(model, currentClient, out isSuccess);

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

        private void SendHisMessage(WebSocketClient client, WebSocket currentSocket)
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
                            SenderName = m.SenderName,
                            TargetId = m.TargetId,
                            MsgType = (MsgType)m.MsgType,
                            Msg = m.Msg,
                            SendDate = m.SendDate
                        };
                        client.SendMsg2Self<string>(JsonConvert.SerializeObject(message), currentSocket);
                    });
                }
            }
        }

        private void Save2DataBase(string senderId, string senderName, string targetId, MsgType msgType, string msg)
        {
            using (var db = new NetApiContextForMsg(_conf))
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (msgType == MsgType.Img)
                        {
                            string fileLocation = "Test/";
                            List<string> paramData = new List<string>() { msg };

                            HttpWebRequest request = WebRequest.Create(string.Format(_conf["Appsettings:UploadBase64Files2FileSystem"], fileLocation)) as HttpWebRequest;
                            request.Method = "Post".ToUpperInvariant();
                            request.ContentType = "application/json";
                            byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(paramData));
                            request.ContentLength = buffer.Length;
                            request.GetRequestStream().Write(buffer, 0, buffer.Length);

                            using (HttpWebResponse resp = request.GetResponse() as HttpWebResponse)
                            {
                                using (StreamReader stream = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                                {
                                    string result = stream.ReadToEnd();
                                    stream.Close();
                                    db.UserMsg.Add(new UserMsg
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        SenderId = senderId,
                                        SenderName = senderName,
                                        TargetId = targetId,
                                        MsgType = (sbyte)msgType,
                                        Msg = JsonConvert.DeserializeObject<List<string>>(result).FirstOrDefault(),
                                    });
                                }
                            }
                        }
                        else
                        {
                            db.UserMsg.Add(new UserMsg
                            {
                                Id = Guid.NewGuid().ToString(),
                                SenderId = senderId,
                                SenderName = senderName,
                                TargetId = targetId,
                                MsgType = (sbyte)msgType,
                                Msg = msg,
                            });
                        }
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

        private void MessageRoute(WebSocketClientModel message, WebSocket currentSocket, out bool isSuccess)
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
                        var client = GetSender(message.SenderId, message.TargetId);
                        if (client != null && !string.IsNullOrEmpty(client.TargetId))
                        {
                            Save2DataBase(client.Id, message.SenderName, client.TargetId, message.MsgType, message.Msg);

                            var clients = GetByTargetId(client.TargetId);
                            if (clients.Any())
                            {
                                //群组广播发送
                                var clientList = clients.ToList();
                                clientList.ForEach(c =>
                                {
                                    c.SendMsg<string>(JsonConvert.SerializeObject(message), currentSocket);
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
                    case MsgType.HeartCheck:
                        var clientHeartCheck = GetSender(message.SenderId, message.TargetId);
                        if (clientHeartCheck != null)
                        {
                            message.SenderId = "system";
                            message.SenderName = "system";
                            message.MsgType = MsgType.HeartCheck;
                            message.Msg = "heartCheck";
                            clientHeartCheck.SendMsg2Self(JsonConvert.SerializeObject(message), currentSocket);
                        }
                        break;
                    case MsgType.System:
                        Save2DataBase(message.SenderId, message.SenderName, message.TargetId, message.MsgType, message.Msg);

                        var clientSys = GetByTargetId(message.TargetId);
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
                        var extClient = GetSender(message.SenderId, message.TargetId);
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
        /// 心跳检测
        /// </summary>
        HeartCheck = 6,

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
        /// websocket实例数组
        /// </summary>
        public List<WebSocket> socketList { get; set; }

        /// <summary>
        /// 发送消息（除自己）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <param name="currentSocket"></param>
        public void SendMsg<T>(T msg, WebSocket currentSocket)
        {
            if (typeof(T).Equals(typeof(string)))
            {
                if (socketList.Count > 0)
                {
                    var otherSocket = socketList.Where(m => m != currentSocket);
                    if (otherSocket.Any())
                    {
                        otherSocket.ToList().ForEach(m =>
                        {
                            if (m.State == WebSocketState.Open)
                            {
                                var msgs = Encoding.UTF8.GetBytes(msg as string);
                                m.SendAsync(new ArraySegment<byte>(msgs), WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                        });
                    };

                }
            }
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendMsg<T>(T msg)
        {
            if (typeof(T).Equals(typeof(string)))
            {
                if (socketList.Count > 0)
                {
                    socketList.ToList().ForEach(m =>
                    {
                        if (m.State == WebSocketState.Open)
                        {
                            var msgs = Encoding.UTF8.GetBytes(msg as string);
                            m.SendAsync(new ArraySegment<byte>(msgs), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 心跳检测
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="currentSocket"></param>
        public void SendMsg2Self<T>(T msg, WebSocket currentSocket)
        {
            if (typeof(T).Equals(typeof(string)))
            {
                var msgs = Encoding.UTF8.GetBytes(msg as string);
                currentSocket.SendAsync(new ArraySegment<byte>(msgs), WebSocketMessageType.Text, true, CancellationToken.None);
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
