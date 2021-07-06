﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace NetApi.Models
{
    /// <summary>
    /// 部门
    /// </summary>
    public partial class departments
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 父键
        /// </summary>
        public int? ParentId { get; set; }
        /// <summary>
        /// 部门编号
        /// </summary>
        [StringLength(100)]
        public string Code { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        [StringLength(100)]
        public string Name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(1000)]
        public string Remarks { get; set; }
    }
}