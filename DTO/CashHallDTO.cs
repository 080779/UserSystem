using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class CashHallDTO
    {
        public long TradeId { get; set; }
        public int TradeType { get; set; }
        public string TradeName { get; set; }// 
        public int Num { get; set; }// 
        public decimal Price { get; set; }// 
        public decimal Amount { get; set; }
        public int CurrencyType { get; set; }// 
        public string CurrencyName { get; set; }// 

        public DateTime CreateTime { get; set; }
    }
    public class CashTradeRecordDTO
    {
        public long TradeId { get; set; }
        public int TradeType { get; set; }
        public string TradeName { get; set; }// 
        public int CurrencyType { get; set; }
        public string CurrencyName { get; set; }
        public int Num { get; set; }// 
        public int Balance { get; set; }// 
        public decimal Price { get; set; }// 
        public int StateType { get; set; } //0:进行中，1：已完成，2：已关闭
        public string StateName { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class CashTradeDetailsDTO
    {
        public string OrderCode { get; set; }
        public long TradeId { get; set; }
        public int TradeType { get; set; }
        public string TradeName { get; set; }// 
        public int CurrencyType { get; set; }
        public string CurrencyName { get; set; }
        public int Num { get; set; }// 
        public int Balance { get; set; }// 
        public decimal Price { get; set; }// 
        public decimal Amount { get; set; }
        public int StateType { get; set; } //0:进行中，1：已完成，2：已关闭
        public string StateName { get; set; }
        public DateTime CreateTime { get; set; }

    }

    public class CashTradeOrderDTO
    {
        public int Num { get; set; }// 
        public decimal Amount { get; set; }
        public DateTime CreateTime { get; set; }
        public string Trader { get; set; }
    }

    public class CashOrderRecordDTO
    {
        public string OrderType { get; set; }
        public int Num { get; set; }// 
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreateTime { get; set; }
        public string Trader { get; set; }
    }
}
