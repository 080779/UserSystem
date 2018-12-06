using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class MemberConfig : EntityTypeConfiguration<MemberEntity>
    {
        public MemberConfig()
        {
            ToTable("tb_members");
            Property(p => p.Name).HasMaxLength(50).IsRequired();
            Property(p => p.Mobile).HasMaxLength(20).IsRequired();
        }
    }
}
