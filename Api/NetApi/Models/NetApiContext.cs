using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NetApi.Models
{
    public partial class NetApiContext : DbContext
    {
        public NetApiContext()
        {
        }

        public NetApiContext(DbContextOptions<NetApiContext> options)
            : base(options)
        {
        }

        public virtual DbSet<authorizations> authorizations { get; set; }
        public virtual DbSet<company> company { get; set; }
        public virtual DbSet<departments> departments { get; set; }
        public virtual DbSet<menus> menus { get; set; }
        public virtual DbSet<positions> positions { get; set; }
        public virtual DbSet<projects> projects { get; set; }
        public virtual DbSet<users> users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=localhost;port=3306;user=root;password=123;database=netapi", x => x.ServerVersion("8.0.22-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<authorizations>(entity =>
            {
                entity.HasComment("权限表");

                entity.Property(e => e.AuthType)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<company>(entity =>
            {
                entity.HasComment("所属公司");

                entity.Property(e => e.Name)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TexNumber)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<departments>(entity =>
            {
                entity.HasComment("部门");

                entity.Property(e => e.Id).HasComment("自增主键");

                entity.Property(e => e.Code)
                    .HasComment("部门编号")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasComment("部门名称")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ParentId).HasComment("父键");

                entity.Property(e => e.Remarks)
                    .HasComment("备注")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<menus>(entity =>
            {
                entity.HasComment("菜单");

                entity.Property(e => e.Id).HasComment("菜单自增主键");

                entity.Property(e => e.Name)
                    .HasComment("菜单名称")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<positions>(entity =>
            {
                entity.HasComment("岗位");

                entity.Property(e => e.Id).HasComment("岗位自增主键");

                entity.Property(e => e.Code)
                    .HasComment("岗位编号")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasComment("岗位名称")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ParentId).HasComment("父键");

                entity.Property(e => e.Remarks)
                    .HasComment("备注")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<projects>(entity =>
            {
                entity.HasComment("所属项目");

                entity.Property(e => e.Code)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<users>(entity =>
            {
                entity.HasComment("用户");

                entity.Property(e => e.Id).HasComment("自增主键");

                entity.Property(e => e.Account)
                    .HasComment("用户名")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Pwd)
                    .HasComment("密码")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Remarks)
                    .HasComment("备注")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
