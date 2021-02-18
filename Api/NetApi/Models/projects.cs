using System;
using System.Collections.Generic;

namespace NetApi.Models
{
    public partial class projects
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public sbyte? Status { get; set; }
    }
}
