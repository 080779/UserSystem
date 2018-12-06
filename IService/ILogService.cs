using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 日志管理接口
    /// </summary>
    public interface ILogService : IServiceSupport
    {
        Task<long> AddAsync(long userId, string logName, string logCode, string description);
        Task<LogSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class LogSearchResult
    {
        public LogDTO[] log { get; set; }
        public long PageCount { get; set; }
    }
}
