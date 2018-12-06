using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 买入实体类
    /// </summary>
    public class CashBuyEntity : BaseEntity
    {
        public long UserId { get; set; }
        public virtual UserEntity Buyer { get; set; }
        public string OrderCode { get; set; }
        public int Number { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public decimal Charge { get; set; }
        public int BalanceNum { get; set; }
        /// <summary>
        /// --  -1:撤销，0，挂买中，1：已完成
        /// </summary>
        public int StateType{ get; set; }
     //   public virtual IdNameEntity Status { get; set; }//订单状态
        public int CurrencyType { get; set; }
        public int BuyType { get; set; }
    }
}
