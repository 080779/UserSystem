using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 日志管理接口
    /// </summary>
    public interface ICashSellService : IServiceSupport
    {
        Task<long> SellToAsync(long userId, long buyId, int sellNum);
        Task<long> AddAsync(long userId, string orderCode, int number, decimal price, decimal amount, decimal charge, int balanceNum, int stateType);
        Task<CashSellSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<CashSellSearchResult> SellListAsync(long? userId, int pageIndex, int pageSize);


    }
    public class CashSellSearchResult
    {
        public CashSellDTO[] log { get; set; }
        public long PageCount { get; set; }
        public long TotalCount { get; set; }
    }
}
