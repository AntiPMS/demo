using System;
using System.Collections.Generic;

namespace NetApi.Models
{
    public partial class authorizations
    {
        public int Id { get; set; }
        public int? MenuId { get; set; }
        public string AuthType { get; set; }
        public int? TypeId { get; set; }
    }
}
