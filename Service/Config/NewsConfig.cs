using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class NewsConfig : EntityTypeConfiguration<NewsEntity>
    {
        public NewsConfig()
        {
            ToTable("tb_news");
            Property(p => p.Code).HasMaxLength(100);
            Property(p => p.Creator).HasMaxLength(50);
            Property(p => p.Content).HasMaxLength(2048);
            Property(p => p.Url).HasMaxLength(256);
        }
    }
}
