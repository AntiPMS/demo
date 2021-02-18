using System;
using System.Collections.Generic;

namespace NetApi.Models
{
    public partial class users
    {
        public int Id { get; set; }
        public string Account { get; set; }
        public string Pwd { get; set; }
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
