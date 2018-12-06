using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 交易订单管理接口
    /// </summary>
    public interface ICashOrderService : IServiceSupport
    {
        Task<long> AddAsync(long buyId, long sellId, long buyUserId, long sellUserId, string orderCode, int number, decimal price,
            decimal amount, int payStateType, int confirmStateType, int stateType);
        Task<CashOrderSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<CashOrderDTO> GetModelAsync(long id);
        Task<CashOrderSearchResult> GetPaymentsListAsync(long? orderId, int pageIndex, int pageSize);
    }
    public class CashOrderSearchResult
    {
        public CashOrderDTO[] log { get; set; }
        public long PageCount { get; set; }
        public long TotalCount { get; set; }
    }
}
