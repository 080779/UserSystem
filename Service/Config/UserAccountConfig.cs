using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class UserAccountConfig : EntityTypeConfiguration<UserAccountEntity>
    {
        public UserAccountConfig()
        {
            ToTable("tb_userAccount");

           // HasRequired(p => p.User).WithMany().HasForeignKey(p => p.UserId).WillCascadeOnDelete(false);
        }
    }
}
