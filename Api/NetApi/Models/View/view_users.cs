using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetApi.Models.View
{
    /// <summary>
    /// 用户查询视图
    /// </summary>
    public class view_users
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
}
