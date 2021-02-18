using System;
using System.Collections.Generic;

namespace NetApi.Models
{
    public partial class departments
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Remarks { get; set; }
    }
}
