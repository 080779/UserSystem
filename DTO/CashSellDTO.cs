using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class CashSellDTO : BaseDTO
    {
        public long UserId { get; set; }
        public string SellerUserCode { get; set; }
        public string SellerUserName { get; set; }
        public string OrderCode { get; set; }
        public int Number { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public decimal Charge { get; set; }
        public int BalanceNum { get; set; }
        /// <summary>
        /// --  -1:撤销，0，挂买中，1：已完成
        /// </summary>
        public int StateType { get; set; }
        public string StateName { get; set; }//订单状态
        public int CurrencyType { get; set; }
        public string CurrencyName { get; set; }
        public int SellType { get; set; }
    }
}
