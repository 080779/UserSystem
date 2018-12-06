using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface ICashHallService : IServiceSupport
    {
        Task<CashHallSearchResult> GetListAsync(int accountType, int pageIndex, int pageSize);
        Task<CashInfoDTO> GetInfoAsync();
        Task<decimal> GetExchangeAsync();
        Task<CashTradeRecordSearchResult> GetTradeRecordAsync(long userId, int pageIndex, int pageSize);
        Task<CashTradeDetailsDTO> GetTradeDetailsAsync(long userId, long tradeId, int tradeType);
        Task<CashTradeOrderSearchResult> GetTradeOrderAsync(long userId, long tradeId, int tradeType, int pageIndex, int pageSize);
        Task<CashOrderRecordSearchResult> GetOrderRecordAsync(long userId, int pageIndex, int pageSize);
        Task<int> GetTradeCloseAsync(long userId, long tradeId, int tradeType);
    }

    public class CashHallSearchResult
    {
        public CashHallDTO[] List { get; set; }
        public long PageCount { get; set; }
    }
    public class CashTradeRecordSearchResult
    {
        public CashTradeRecordDTO[] List { get; set; }
        public long PageCount { get; set; }
    }
    public class CashTradeOrderSearchResult
    {
        public CashTradeOrderDTO[] List { get; set; }
        public long PageCount { get; set; }
    }
    public class CashOrderRecordSearchResult
    {
        public CashOrderRecordDTO[] List { get; set; }
        public long PageCount { get; set; }
    }
    public class CashInfoResult
    {
        public CashInfoDTO[] Info { get; set; }
    }
}
