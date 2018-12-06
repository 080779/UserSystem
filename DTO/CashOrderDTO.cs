using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class CashOrderDTO : BaseDTO
    {
        public string OrderCode { get; set; }
        public long BuyId { get; set; }
        public long SellId { get; set; }
        public long BuyUserId { get; set; }

        public long SellUserId { get; set; }
        public string BuyerCode { get; set; }
        public string SellerCode { get; set; }

        public int Number { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        /// <summary>
        /// 1:撤销，0，交易中，1：已支付
        /// </summary>
        public int PayStateType { get; set; }
        public string PayStateName { get; set; }
        public DateTime? PayTime { get; set; }
        /// <summary>
        /// 1:撤销，0，交易中，1：已支付
        /// </summary>
        public int ConfirmStateType { get; set; }
        public string ConfirmStateName { get; set; }
        public DateTime? ConfirmTime { get; set; }
        /// <summary>
        /// 1:撤销，0，挂卖中，1：已完成
        /// </summary>
        public int StateType { get; set; }

        public string StateName { get; set; }
        public string EthAddress { get; set; }

    }
}
