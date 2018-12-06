using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class CashSellConfig : EntityTypeConfiguration<CashSellEntity>
    {
        public CashSellConfig()
        {
            ToTable("tb_cashSell");
            Property(p => p.OrderCode).HasMaxLength(50).IsRequired();
            Property(p => p.Price).HasPrecision(18, 2);
            Property(p => p.Amount).HasPrecision(18, 2);
            Property(p => p.Charge).HasPrecision(18, 2);
            HasRequired(p => p.Seller).WithMany().HasForeignKey(p => p.UserId).WillCascadeOnDelete(false);
          //  HasRequired(p => p.Status).WithMany().HasForeignKey(p => p.StateType).WillCascadeOnDelete(false);
        }
    }
}
