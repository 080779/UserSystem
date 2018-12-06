using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class RechargeConfig : EntityTypeConfiguration<RechargeEntity>
    {
        public RechargeConfig()
        {
            ToTable("tb_recharge");
            HasRequired(p => p.User).WithMany().HasForeignKey(p => p.UserId).WillCascadeOnDelete(false);
        }
    }
}
