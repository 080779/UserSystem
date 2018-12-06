using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 用户每天奖金记录接口
    /// </summary>
    public interface IUserAccountService : IServiceSupport
    {
        Task<long> AddAsync(long userId, decimal amount, decimal bonusAmount);
        Task<bool> DeleteAsync(long id);
        Task<UserAccountDTO> GetModelAsync(long id);
        Task<UserAccountDTO> GetDefaultModelAsync(long userId);
        Task<UserAccountSearchResult> GetModelListAsync(long? userId,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<UserAccountSearchResult01> GetModelListAsync(DateTime? time, int pageIndex, int pageSize);
    }
    public class UserAccountSearchResult
    {
        public UserAccountDTO[] List { get; set; }
        public long PageCount { get; set; }
    }
    public class UserAccountSearchResult01
    {
        public decimal TotalBonusAmount { get; set; }
        public long PageCount { get; set; }
        public CalcUserAccountDTO[] List { get; set; }
    }
}
