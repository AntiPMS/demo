﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace NetApi.Models
{
    public partial class pxusers
    {
        [Key]
        public int id { get; set; }
        public int uid { get; set; }
        [Required]
        [StringLength(100)]
        public string userName { get; set; }
        public int latest_id { get; set; }
        [Required]
        [StringLength(255)]
        public string path { get; set; }
    }
}