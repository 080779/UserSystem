using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 课程订单接口
    /// </summary>
    public interface ICourseOrderService : IServiceSupport
    {
        Task<long> AddAsync(long buyerId, string buyerName,long courseId,string imgUrl);
        Task<long> AddAsync(long buyerId, string buyerName, long courseId);
        Task<bool> AuditAsync(long id,int stateId, long auditorId);
        Task<CourseOrderSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class CourseOrderSearchResult
    {
        public CourseOrderDTO[] CourseOrders { get; set; }
        public long PageCount { get; set; }
    }
}
