using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class CourseOrderConfig : EntityTypeConfiguration<CourseOrderEntity>
    {
        public CourseOrderConfig()
        {
            ToTable("tb_courseOrders");
            Property(p => p.Code).HasMaxLength(50).IsRequired();
            Property(p => p.BuyerName).HasMaxLength(50);
            Property(p => p.CourseName).HasMaxLength(50);
            Property(p => p.AuditMobile).HasMaxLength(50);
            Property(p => p.ImgUrl).HasMaxLength(256);
        }
    }
}
