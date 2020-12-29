using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetApi.Models
{
    public partial class projects
    {
        [Key]
        public int Id { get; set; }
        public int? ParentId { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string Code { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string Name { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndDate { get; set; }
        public sbyte? Status { get; set; }
    }
}
