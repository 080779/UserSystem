using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class SettingTypeConfig : EntityTypeConfiguration<SettingTypeEntity>
    {
        public SettingTypeConfig()
        {
            ToTable("tb_settingTypes");
            Property(p => p.Name).HasMaxLength(50).IsRequired();
            Property(p => p.Description).HasMaxLength(100);
        }
    }
}
