using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetApi.Models.View
{
    public class RequestViewUsers
    {
        public string Account { get; set; }
        public string Pwd { get; set; }
    }

    /// <summary>
    /// 新增用户
    /// </summary>
    public class RequestInsertUsers
    {
        /// <summary>
        /// 帐户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public sbyte? Sex { get; set; }
    }

    /// <summary>
    /// 修改用户
    /// </summary>
    public class RequestUpdateUsers
    {
        /// <summary>
        /// 帐户
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 帐户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public sbyte? Sex { get; set; }
    }

    /// <summary>
    /// 发送消息模型
    /// </summary>
    public class MsgUserInfo
    {
        /// <summary>
        /// 目标主键(咨询主键)
        /// </summary>
        public string userId { get; set; }

        ///// <summary>
        ///// 纯文本消息=1、图片=2、系统发送的消息=7
        ///// </summary>
        //public MiraiMsgType msgType { get; set; }

        /// <summary>
        /// 具体消息
        /// </summary>
        public SystemInfo msg { get; set; }
    }

    public class SystemInfo
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; set; }
    }

    /// <summary>
    /// 上传base64文件 入参模型
    /// </summary>
    public class Base64FileModel
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// base64串
        /// </summary>
        public string content { get; set; }
    }
}
