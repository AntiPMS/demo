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
            optionsBuilder.UseMySql(_conf.GetConnectionString("NetApiConnection"), new MySqlServerVersion("8.0.25"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");

            modelBuilder.Entity<UserMsg>(entity =>
            {
                entity.HasComment("用户历史消息表");

                entity.Property(e => e.IsRead)
                    .HasDefaultValueSql("'0'")
                    .HasComment("已读：0=未读 1=已读 ");

                entity.Property(e => e.MsgType).HasComment("消息类型：加入=0  纯文本=1  图片=2  断开连接=9");

                entity.Property(e => e.SendDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                    .HasComment("发送日期");

                entity.Property(e => e.SenderId).HasComment("发送人Id");

                entity.Property(e => e.SenderName)
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.TargetId).HasComment("接收者Id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
