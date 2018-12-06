using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 交易订单实体类
    /// </summary>
    public class CashOrderEntity : BaseEntity
    {
        public string OrderCode { get; set; }
        public long BuyId { get; set; }
        public virtual CashBuyEntity BuyOrder { get; set; }
        public long SellId { get; set; }
        public virtual CashSellEntity SellOrder { get; set; }
        public long BuyUserId { get; set; }

        public long SellUserId { get; set; }

        public int Number { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        /// <summary>
        /// 1:撤销，0，交易中，1：已支付
        /// </summary>
        public int PayStateType { get; set; }
       // public virtual IdNameEntity PayState { get; set; }
        public DateTime? PayTime { get; set; }
        /// <summary>
        /// 1:撤销，0，交易中，1：已收款
        /// </summary>
        public int ConfirmStateType { get; set; }
       // public virtual IdNameEntity ConfirmState { get; set; }
        public DateTime? ConfirmTime { get; set; }
        /// <summary>
        /// 1:撤销，0，挂卖中，1：已完成
        /// </summary>
        public int StateType { get; set; }

      //  public virtual IdNameEntity State { get; set; }
    }
}
