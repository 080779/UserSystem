using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class AdminLogConfig:EntityTypeConfiguration<AdminLogEntity>
    {
        public AdminLogConfig()
        {
            ToTable("tb_adminLogs");
            Property(a => a.AdminMobile).HasMaxLength(50);
            Property(a => a.PermissionTypeName).HasMaxLength(50);
            Property(a => a.Description).HasMaxLength(500);
            Property(a => a.IpAddress).HasMaxLength(50);
            Property(a => a.Tip).HasMaxLength(500);
        }
    }
}
