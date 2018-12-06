using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Config
{
    class LinkConfig : EntityTypeConfiguration<LinkEntity>
    {
        public LinkConfig()
        {
            ToTable("tb_links");
            Property(n => n.Name).HasMaxLength(50).IsRequired();
            Property(n => n.ImgUrl).HasMaxLength(256);
            Property(n => n.Url).HasMaxLength(256);
            Property(n => n.Tip).HasMaxLength(256);
            Property(n => n.TypeName).HasMaxLength(50);
        }
    }
}
