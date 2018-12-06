using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 交易买入接口
    /// </summary>
    public interface ICashBuyService : IServiceSupport
    {
        Task<long> BuyFromAsync(long userId,long buyerUid, long sellId, int buyNum);
        Task<long> AddAsync(long userId, long buyerUid, string orderCode, int number, decimal price, decimal amount, decimal charge, int balanceNum, int stateType);
        Task<CashBuySearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<CashBuySearchResult> BuyListAsync(long? userId, int pageIndex, int pageSize);
    }
    public class CashBuySearchResult
    {
        public CashBuyDTO[] log { get; set; }
        public long PageCount { get; set; }
        public long TotalCount { get; set; }
    }
}
