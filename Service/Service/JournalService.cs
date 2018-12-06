using IMS.Common.Enums;
using IMS.DTO;
using IMS.IService;
using IMS.Service.Entity;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Service
{
    public class JournalService : IJournalService
    {
        private static ILog log = LogManager.GetLogger(typeof(OrderService));
        public JournalDTO ToDTO(JournalEntity entity)
        {
            JournalDTO dto = new JournalDTO();
            dto.BalanceAmount = entity.BalanceAmount;
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.InAmount = entity.InAmount;
            dto.JournalTypeId = entity.JournalTypeId;
            dto.JournalTypeName = entity.JournalTypeId.GetEnumName<JournalTypeEnum>();
            dto.Amount = entity.InAmount > 0 ? (decimal)entity.InAmount : (decimal)entity.OutAmount;
            dto.OutAmount = entity.OutAmount;
            dto.Remark = entity.Remark;
            dto.RemarkEn = entity.RemarkEn;
            dto.UserId = entity.UserId;
            dto.Mobile = entity.User.Mobile;
            dto.NickName = entity.User.NickName;
            dto.OrderCode = entity.OrderCode;
            dto.IsEnabled = entity.IsEnabled;
            dto.LevelId = entity.LevelId;
            dto.GoodsId = entity.GoodsId;
            return dto;
        }
        public JournalADTO ToDTOA(JournalEntity entity)
        {
            JournalADTO dto = new JournalADTO();
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;

            dto.JournalTypeName = entity.JournalTypeId.GetEnumName<JournalTypeEnum>();
            dto.Amount = entity.InAmount > 0 ? "+" + ((decimal)entity.InAmount) : (0 - (decimal)entity.OutAmount).ToString();
            dto.BalanceAmount = entity.BalanceAmount;
            return dto;
        }
        public JournalPartDTO ToPartDTO(JournalEntity entity)
        {
            JournalPartDTO dto = new JournalPartDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.Remark = entity.Remark;
            dto.JournalTypeName = entity.JournalTypeId.GetEnumName<JournalTypeEnum>();
            dto.Amount = entity.InAmount > 0 ? "+" + ((decimal)entity.InAmount) : (0 - (decimal)entity.OutAmount).ToString();
            dto.BalanceAmount = entity.BalanceAmount;
            return dto;
        }
        public async Task<JournalSearchResult> GetModelListAsync(long? userId, long? journalTypeId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                JournalSearchResult result = new JournalSearchResult();
                var entities = dbc.GetAll<JournalEntity>().AsNoTracking().Where(j=>j.IsEnabled==true);
                if (userId != null)
                {
                    entities = entities.Where(a => a.UserId == userId);
                }
                if (journalTypeId != null)
                {
                    entities = entities.Where(a => a.JournalTypeId == journalTypeId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Remark.Contains(keyword) || g.User.Mobile.Contains(keyword) || g.User.NickName.Contains(keyword) || g.OrderCode.Contains(keyword));
                }
                if (startTime != null)
                {
                    entities = entities.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    entities = entities.Where(a => SqlFunctions.DateDiff("day", endTime, a.CreateTime) <= 0);
                }
                result.PageCount = (int)Math.Ceiling((await entities.LongCountAsync()) * 1.0f / pageSize);
                decimal? totalInAmount = await entities.SumAsync(j => j.InAmount);
                decimal? totalOutAmount= await entities.SumAsync(j => j.OutAmount);
                result.TotalInAmount = totalInAmount == null ? 0 : totalInAmount;
                result.TotalOutAmount = totalOutAmount == null ? 0 : totalOutAmount;
                var journalResult = await entities.Include(j => j.User).OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Journals = journalResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }
        public async Task<JournalResult> GetAccoutAListAsync(long? userId, int currencyType, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                JournalResult result = new JournalResult();
                var entities = dbc.GetAll<JournalEntity>().AsNoTracking().Where(j => j.IsEnabled == true);
                if (userId != null)
                {
                    entities = entities.Where(a => a.UserId == userId);
                }
                if (currencyType > 0)
                {
                    entities = entities.Where(a => a.CurrencyType == currencyType);
                }

                result.PageCount = (int)Math.Ceiling((await entities.LongCountAsync()) * 1.0f / pageSize);
                var journalResult = await entities.Include(j => j.User).OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.List = journalResult.Select(a => ToDTOA(a)).ToArray();
                return result;
            }
        }
        public async Task<JournalPartSearchResult> GetListAsync(long? userId, int currencyType,string findKey, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                JournalPartSearchResult result = new JournalPartSearchResult();
                try
                {
                    var entities = dbc.GetAll<JournalEntity>().AsNoTracking().Where(j => j.IsEnabled == true);
                    if (userId != null)
                    {
                        entities = entities.Where(a => a.UserId == userId);
                    }
                    if (currencyType > 0)
                    {
                        entities = entities.Where(a => a.CurrencyType == currencyType);
                    }
                    if (!string.IsNullOrWhiteSpace(findKey))
                        entities = entities.Where(a => a.Remark.Contains(findKey));

                    result.TotalCount = await entities.LongCountAsync();
                    result.PageCount = (int)Math.Ceiling((result.TotalCount) * 1.0f / pageSize);
                    var journalResult = await entities.Include(j => j.User).OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                    result.List = journalResult.Select(a => ToPartDTO(a)).ToArray();
                }
                catch (Exception ex)
                {
                    log.DebugFormat($"journalSercive.GetListAsync:{ex.ToString()}");
                }
                return result;
            }
        }
        /// <summary>
        /// 加入流水表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="Amount"></param>
        /// <param name="journalTypeId"></param>
        /// <param name="currencyType"></param>
        /// <param name="balanceAmount"></param>
        /// <param name="remark"></param>
        /// <param name="type">1-加，0-减</param>
        /// <returns></returns>
        public async Task<bool> AddAsync(long userId,decimal Amount ,int journalTypeId,int currencyType,decimal balanceAmount, string remark,int type)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                JournalEntity journal = new JournalEntity();
                if (type == 1)//1-加，0-减
                {
                    journal.InAmount = Amount;
                    journal.OutAmount = 0;
                }
                else
                {
                    journal.InAmount = 0;
                    journal.OutAmount = Amount;
                }
                journal.JournalTypeId = journalTypeId;
                journal.CurrencyType = currencyType;
                journal.Remark = remark;
                journal.UserId = userId;
                journal.BalanceAmount = balanceAmount;
                dbc.Journals.Add(journal);

                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
