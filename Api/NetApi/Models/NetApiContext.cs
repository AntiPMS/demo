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

        public virtual DbSet<UserMsg> UserMsg { get; set; }
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
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserMsg>(entity =>
            {
                entity.HasComment("用户历史消息表");

                entity.Property(e => e.Id)
                    .HasColumnType("varchar(36)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.IsRead)
                    .HasDefaultValueSql("'0'")
                    .HasComment("已读：0=未读 1=已读 ");

                entity.Property(e => e.Msg)
                    .HasColumnType("mediumtext")
                    .HasComment("消息文本")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.MsgType).HasComment("消息类型：加入=0  纯文本=1  图片=2  断开连接=9");

                entity.Property(e => e.SendDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("发送日期");

                entity.Property(e => e.SenderId)
                    .HasColumnType("varchar(100)")
                    .HasComment("发送人Id")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TargetId)
                    .HasColumnType("varchar(100)")
                    .HasComment("接收者Id")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<authorizations>(entity =>
            {
                entity.HasComment("权限表");

                entity.Property(e => e.AuthType)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<company>(entity =>
            {
                entity.HasComment("所属公司");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TexNumber)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<departments>(entity =>
            {
                entity.HasComment("部门");

                entity.Property(e => e.Id).HasComment("自增主键");

                entity.Property(e => e.Code)
                    .HasColumnType("varchar(100)")
                    .HasComment("部门编号")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(100)")
                    .HasComment("部门名称")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ParentId).HasComment("父键");

                entity.Property(e => e.Remarks)
                    .HasColumnType("varchar(1000)")
                    .HasComment("备注")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<menus>(entity =>
            {
                entity.HasComment("菜单");

                entity.Property(e => e.Id).HasComment("菜单自增主键");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(100)")
                    .HasComment("菜单名称")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<positions>(entity =>
            {
                entity.HasComment("岗位");

                entity.Property(e => e.Id).HasComment("岗位自增主键");

                entity.Property(e => e.Code)
                    .HasColumnType("varchar(100)")
                    .HasComment("岗位编号")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(100)")
                    .HasComment("岗位名称")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ParentId).HasComment("父键");

                entity.Property(e => e.Remarks)
                    .HasColumnType("varchar(1000)")
                    .HasComment("备注")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<projects>(entity =>
            {
                entity.HasComment("所属项目");

                entity.Property(e => e.Code)
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.StartDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<users>(entity =>
            {
                entity.HasComment("用户");

                entity.Property(e => e.Id).HasComment("自增主键");

                entity.Property(e => e.Account)
                    .HasColumnType("varchar(100)")
                    .HasComment("用户名")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Pwd)
                    .HasColumnType("varchar(100)")
                    .HasComment("密码")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Remarks)
                    .HasColumnType("varchar(1000)")
                    .HasComment("备注")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
