using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class LogConfig : EntityTypeConfiguration<LogEntity>
    {
        public LogConfig()
        {
            ToTable("tb_logs");
            Property(p => p.LogName).HasMaxLength(30).IsRequired();
            Property(p => p.LogCode).HasMaxLength(50).IsRequired();
            Property(p => p.Description).HasMaxLength(100);
        }
    }
}
