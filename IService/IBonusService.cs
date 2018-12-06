using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 奖金接口
    /// </summary>
    public interface IBonusService : IServiceSupport
    {
        Task<long> AddAsync(long userId, decimal amount, decimal revenue, decimal sf, string source, long fromUserID, int isSettled);
        
        Task<bool> DeleteAsync(long id);
        Task<BonusDTO> GetModelAsync(long id);
        Task<BonusDTO> GetDefaultModelAsync(long userId);
        Task<BonusSearchResult> GetModelListAsync(long? userId,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<BonusSearchResult> BoussListAsync(long? userId, long? typeId, int pageIndex, int pageSize);
        Task<bool> CalcAwardRecommend(long orderId, string orderCode, decimal amount, long buyerId );
        Task<bool> CalcAwardBuy(long orderId, string orderCode, decimal profit, long buyerId );
        Task<bool> CalcStaticAward(long orderId, string orderCode, long buyerId);
        Task<bool> CalcAwardLevelDiff(long orderId, string orderCode, decimal amount, long buyerId);
        Task<bool> CalcAwardLevelDiffMaxAvg(long orderId, string orderCode, decimal amount, long buyerId, string buyMobile);
    }
    public class BonusSearchResult
    {
        public BonusDTO[] List { get; set; }
        public long PageCount { get; set; }
    }

}
