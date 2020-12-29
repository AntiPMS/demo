using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetApi.Models
{
    public partial class authorizations
    {
        [Key]
        public int Id { get; set; }
        public int? MenuId { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string AuthType { get; set; }
        public int? TypeId { get; set; }
    }
}
