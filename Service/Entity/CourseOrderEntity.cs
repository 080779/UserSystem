using IMS.Service;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 课程订单实体类
    /// </summary>
    public class CourseOrderEntity : BaseEntity
    {
        public string Code { get; set; }
        public long BuyerId { get; set; }
        public string BuyerName { get; set; }
        public long CourseId { get; set; }
        public string CourseName { get; set; }
        public string ImgUrl { get; set; }//打款凭证图
        public decimal Amount { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;
        public int PayTypeId { get; set; } = 1;
        public int OrderStateId { get; set; } = 1;
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditTime { get; set; }//审核时间
        /// <summary>
        /// 审核人账号
        /// </summary>
        public string AuditMobile { get; set; }//审核人账号
    }
}
