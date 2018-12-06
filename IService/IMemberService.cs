using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 成员管理接口
    /// </summary>
    public interface IMemberService : IServiceSupport
    {
        Task<long> AddAsync(long TeamId, string name, string mobile, int AddType);
        Task<bool> UpdateAsync(long id, string name, string mobile);
        Task<bool> DeleteAsync(long id);
        Task<MemberDTO> GetModelAsync(long id);
        Task<MemberDTO> GetModelByMobileAsync(string id);
        Task<MemberSearchResult> GetModelListAsync(long? userId,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class MemberSearchResult
    {
        public MemberDTO[] Member { get; set; }
        public long PageCount { get; set; }
    }
}
