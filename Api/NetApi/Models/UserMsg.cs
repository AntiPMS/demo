using System;
using System.Collections.Generic;

namespace NetApi.Models
{
    public partial class UserMsg
    {
        public string Id { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string TargetId { get; set; }
        public sbyte? MsgType { get; set; }
        public string Msg { get; set; }
        public DateTime? SendDate { get; set; }
        public sbyte? IsRead { get; set; }
    }
}
