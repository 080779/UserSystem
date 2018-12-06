using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class CashInfoDTO
    {
        /// <summary>
        /// 待成交订单量
        /// </summary>
        public int WaitOrderQuantity { get; set; }
        /// <summary>
        /// 日成交量
        /// </summary>
        public int DayOrderQuantity { get; set; }

    }
}
