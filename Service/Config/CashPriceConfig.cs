using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class CashPriceConfig : EntityTypeConfiguration<CashPriceEntity>
    {
        public CashPriceConfig()
        {
            ToTable("tb_CashPrice");
        }
    }
}
