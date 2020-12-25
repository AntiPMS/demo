using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetApi.Models
{
    public partial class users
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string uname { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string pwd { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string remark { get; set; }
    }
}
