using IMS.DTO;
using System;
using System.Threading.Tasks;
namespace IMS.IService
{
   public interface IRechargeService : IServiceSupport
    {
        Task<long> AddAsync(long userId, int currencyType, int rechargeType, decimal rechargeableMoney);
        Task<RechargeSearchResult> GetRechargeListAsync(long? userId, string userCode, int rechargtateId, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class RechargeSearchResult
    {
        public RechargeDTO[] Recharges { get; set; }
        public long PageCount { get; set; }
    }
}
