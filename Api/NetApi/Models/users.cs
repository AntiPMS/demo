﻿using System;
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
        public string Account { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string Pwd { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; }
        public sbyte? Sex { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Birthday { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string Remarks { get; set; }
        public int? CompanyId { get; set; }
        public int? ProjectId { get; set; }
        public int? DeptId { get; set; }
        public int? PositionId { get; set; }
    }
}
