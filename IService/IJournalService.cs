using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface IJournalService:IServiceSupport
    {
        Task<JournalSearchResult> GetModelListAsync(long? userId, long? journalTypeId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<JournalResult> GetAccoutAListAsync(long? userId, int currencyType, int pageIndex, int pageSize);
        Task<JournalPartSearchResult> GetListAsync(long? userId, int currencyType, string findKey, int pageIndex, int pageSize);
        /// <summary>
        /// 加入流水表
        /// </summary>
        /// <param name="userId">1</param>
        /// <param name="Amount">2</param>
        /// <param name="journalTypeId">3</param>
        /// <param name="currencyType">4</param>
        /// <param name="balanceAmount">5</param>
        /// <param name="remark">6</param>
        /// <param name="type">1-加，0-减</param>
        /// <returns></returns>
        Task<bool> AddAsync(long userId, decimal Amount, int journalTypeId, int currencyType, decimal balanceAmount, string remark, int type);
    }
    public class JournalSearchResult
    {
        public JournalDTO[] Journals { get; set; }
        public long PageCount { get; set; }
        public decimal? TotalInAmount { get; set; }
        public decimal? TotalOutAmount { get; set; }
    }
    public class JournalResult
    {
        public JournalADTO[] List { get; set; }
        public long PageCount { get; set; }
 
    }
    public class JournalPartSearchResult
    {
        public JournalPartDTO[] List { get; set; }
        public long PageCount { get; set; }
        public long TotalCount { get; set; }
    }
}
