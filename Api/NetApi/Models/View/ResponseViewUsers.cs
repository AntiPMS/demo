using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetApi.Models.View
{
    /// <summary>
    /// 用户查询视图
    /// </summary>
    public class ResponseViewUsers
    {
        public int Id { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public sbyte? Sex { get; set; }
        public DateTime? Birthday { get; set; }
        public string Remarks { get; set; }
        public int? CompanyId { get; set; }
        public int? ProjectId { get; set; }
        public int? DeptId { get; set; }
        public int? PositionId { get; set; }
    }

    /// <summary>
    /// 消息返参模型
    /// </summary>
    public class userMsg
    {
        /// <summary>
        /// 消息主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 发送人Id
        /// </summary>
        public string SenderId { get; set; }

        /// <summary>
        /// 接收方Id
        /// </summary>
        public string TargetId { get; set; }

        /// <summary>
        /// 消息类型：纯文本消息=1、图片=2、系统发送的消息=7
        /// </summary>
        public sbyte? MsgType { get; set; }

        /// <summary>
        /// 消息主体
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 发送日期
        /// </summary>
        public DateTime? SendDate { get; set; }

        /// <summary>
        /// 已读标志：0=未读，1=已读
        /// </summary>
        public sbyte? IsRead { get; set; }
    }

}
