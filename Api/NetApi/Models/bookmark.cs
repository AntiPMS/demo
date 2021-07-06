﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace NetApi.Models
{
    public partial class bookmark
    {
        [Key]
        public int id { get; set; }
        public int uid { get; set; }
        [Required]
        [StringLength(100)]
        public string userName { get; set; }
        public int pid { get; set; }
        [Required]
        [StringLength(255)]
        public string purl { get; set; }
        [Required]
        [StringLength(255)]
        public string title { get; set; }
        [Required]
        [StringLength(999)]
        public string tag { get; set; }
        public int pageCount { get; set; }
        public sbyte illustType { get; set; }
        public bool is_r18 { get; set; }
        [Column(TypeName = "float(5,3)")]
        public float score { get; set; }
        [Required]
        [StringLength(20)]
        public string illust_level { get; set; }
        public int viewCount { get; set; }
        public int bookmarkCount { get; set; }
        public int likeCount { get; set; }
        public int commentCount { get; set; }
        [Required]
        [StringLength(999)]
        public string urls { get; set; }
        [Required]
        [StringLength(255)]
        public string original { get; set; }
        [Required]
        [StringLength(255)]
        public string path { get; set; }
    }
}