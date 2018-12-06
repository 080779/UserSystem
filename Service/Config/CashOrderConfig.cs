using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class CashOrderConfig : EntityTypeConfiguration<CashOrderEntity>
    {
        public CashOrderConfig()
        {
            ToTable("tb_cashOrder");
            Property(p => p.OrderCode).HasMaxLength(50).IsRequired();
            Property(p => p.Price).HasPrecision(18, 2);
            Property(p => p.Amount).HasPrecision(18, 2);
            HasRequired(p => p.BuyOrder).WithMany().HasForeignKey(p => p.BuyId).WillCascadeOnDelete(false);
            HasRequired(p => p.SellOrder).WithMany().HasForeignKey(p => p.SellId).WillCascadeOnDelete(false);
         //   HasRequired(p => p.PayState).WithMany().HasForeignKey(p => p.PayStateType).WillCascadeOnDelete(false);
        //    HasRequired(p => p.ConfirmState).WithMany().HasForeignKey(p => p.ConfirmStateType).WillCascadeOnDelete(false);
         //   HasRequired(p => p.State).WithMany().HasForeignKey(p => p.StateType).WillCascadeOnDelete(false);

        }
    }
}
