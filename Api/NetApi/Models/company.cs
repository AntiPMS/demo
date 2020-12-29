using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetApi.Models
{
    public partial class company
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string TexNumber { get; set; }
    }
}
