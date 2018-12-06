using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;


namespace IMS.Service.Config
{
    class TransferConfig: EntityTypeConfiguration<TransferEntity>
    {
        public TransferConfig()
        {
            ToTable("tb_change");
          
        }
    }
}
