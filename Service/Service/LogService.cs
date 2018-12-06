using IMS.DTO;
using IMS.IService;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Service
{
    public class LogService : ILogService
    {
        public LogDTO ToDTO(LogEntity entity)
        {
            LogDTO dto = new LogDTO();
            dto.UserId = entity.UserId;
            dto.LogName = entity.LogName;
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.Id = entity.Id;
            dto.LogCode = entity.LogCode;
            return dto;
        }
        public async Task<long> AddAsync(long userId, string logName, string logCode, string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                LogEntity log = new LogEntity();
                log.UserId = userId;
                log.Description = description;
                log.LogName = logName;
                log.LogCode = logCode;
                dbc.Logs.Add(log);
                await dbc.SaveChangesAsync();
                return log.Id;
            }
        }

        public async Task<LogSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                LogSearchResult result = new LogSearchResult();
                var logs = dbc.GetAll<LogEntity>().AsNoTracking();
                if (!string.IsNullOrEmpty(keyword))
                {
                    logs = logs.Where(a => a.LogName.Contains(keyword) || a.Description.Contains(keyword));
                }
               
                if (startTime != null)
                {
                    logs = logs.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    logs = logs.Where(a => SqlFunctions.DateDiff("day", endTime, a.CreateTime) <= 0);
                }
                result.PageCount = (int)Math.Ceiling((await logs.LongCountAsync()) * 1.0f / pageSize);
                var logsResult = await logs.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.log = logsResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }
    }
}
