using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class SettingConfig:EntityTypeConfiguration<SettingEntity>
    {
        public SettingConfig()
        {
            ToTable("tb_settings");
            Property(s => s.ParamName).HasMaxLength(50).IsRequired();
            Property(s => s.Name).HasMaxLength(50).IsRequired();
            Property(s => s.Param).HasMaxLength(256).IsRequired();
            Property(s => s.Remark).HasMaxLength(100);
        }
    }
}
