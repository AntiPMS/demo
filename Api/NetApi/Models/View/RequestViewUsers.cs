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
}
