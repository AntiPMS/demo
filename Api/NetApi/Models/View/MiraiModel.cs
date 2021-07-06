using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetApi.Models.View
{
    public static class MiraiMsgType 
    {
        public static string PlainMessage = "Plain";
        public static string ImageMessage = "Image";
    }

    public class MsgModel
    {
        public string type { get; set; }
        public string content { get; set; }
    }
}

