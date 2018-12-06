using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class CourseOrderDTO : BaseDTO
    {
        public string Code { get; set; }
        public long BuyerId { get; set; }
        public string BuyerName { get; set; }
        public long CourseId { get; set; }
        public string CourseName { get; set; }
        public string ImgUrl { get; set; }
        public decimal Amount { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;
        public int PayTypeId { get; set; } = 1;
        public int OrderStateId { get; set; } = 0;
        public string OrderStateName { get; set; }
        public DateTime? AuditTime { get; set; }//审核时间
        public string AuditMobile { get; set; }//审核人账号
    }
}
