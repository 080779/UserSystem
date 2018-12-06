using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 新闻管理接口
    /// </summary>
    public interface INewsService : IServiceSupport
    {
        Task<long> AddAsync(string code, string content, DateTime failureTime, long creatorId);
        Task<bool> UpdateAsync(long id, string code, string content, DateTime failureTime);
        Task<bool> DeleteAsync(long id);
        Task<NewsDTO> GetModelAsync(long id);
        Task<NewsSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class NewsSearchResult
    {
        public NewsDTO[] News { get; set; }
        public long PageCount { get; set; }
    }   
}
