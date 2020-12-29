using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetApi.Models
{
    public partial class departments
    {
        [Key]
        public int Id { get; set; }
        public int? ParentId { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string Code { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string Remarks { get; set; }
    }
}
