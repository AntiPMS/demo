using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace NetApi.Models
{
    public partial class NetApiContextForMsg : DbContext
    {
        private IConfiguration _conf { get; }

        public NetApiContextForMsg(IConfiguration configuration)
        {
            _conf = configuration;
        }

        public NetApiContextForMsg(DbContextOptions<NetApiContext> options, IConfiguration configuration)
            : base(options)
        {
            _conf = configuration;
        }

        public virtual DbSet<UserMsg> UserMsg { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_conf.GetConnectionString("NetApiConnection"), x => x.ServerVersion("8.0.22-mysql"));
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

                entity.Property(e => e.SenderName)
                   .HasColumnType("varchar(100)")
                   .HasComment("发送人姓名")
                   .HasCharSet("utf8")
                   .HasCollation("utf8_general_ci");

                entity.Property(e => e.TargetId)
                    .HasColumnType("varchar(100)")
                    .HasComment("接收者Id")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
