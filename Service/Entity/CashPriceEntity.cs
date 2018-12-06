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
    /// 积分价格实体类
    /// </summary>
    public class CashPriceEntity : BaseEntity
    {
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal Price { get; set; }
    }
}
