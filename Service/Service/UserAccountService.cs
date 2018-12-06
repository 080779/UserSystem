using IMS.Common;
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
    public class UserAccountService : IUserAccountService
    {
        public UserAccountDTO ToDTO(UserAccountEntity entity)
        {
            UserAccountDTO dto = new UserAccountDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.Amount = entity.Amount;
            dto.BonusAmount = entity.BonusAmount;
          //  dto.UID = entity.User.ShopUID;
            dto.UserId = entity.UserId;
            return dto;
        }
        public async Task<long> AddAsync(long userId, decimal amount, decimal bonusAmount)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserAccountEntity entity = new UserAccountEntity();
                entity.UserId = userId;
                entity.Amount = amount;
                entity.BonusAmount = bonusAmount;
                dbc.UserAccounts.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserAccountEntity entity = await dbc.GetAll<UserAccountEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<UserAccountDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<UserAccountEntity>().AsNoTracking().SingleOrDefaultAsync(a=>a.Id==id);
                if(entity==null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<UserAccountDTO> GetDefaultModelAsync(long userId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<UserAccountEntity>().AsNoTracking().SingleOrDefaultAsync(a => a.UserId == userId);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<UserAccountSearchResult> GetModelListAsync(long? userId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserAccountSearchResult result = new UserAccountSearchResult();
                var entities = dbc.GetAll<UserAccountEntity>().AsNoTracking();
                if (userId != null)
                {
                    entities = entities.Where(a => a.UserId == userId);
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
                var addressResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.List = addressResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }       

        public async Task<UserAccountSearchResult01> GetModelListAsync(DateTime? time, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserAccountSearchResult01 result = new UserAccountSearchResult01();
                var entities = dbc.GetAll<UserAccountEntity>().AsNoTracking();
                if (time != null)
                {
                    entities = entities.Where(a => SqlFunctions.DateDiff("day", time, a.CreateTime) == 0);
                }
                result.PageCount = (int)Math.Ceiling((await entities.LongCountAsync()) * 1.0f / pageSize);
                if(result.PageCount<=0)
                {
                    result.TotalBonusAmount = 0;
                }
                else
                {
                    result.TotalBonusAmount = await entities.SumAsync(a => a.Amount);
                }
                var addressResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.List = addressResult.Select(a => new CalcUserAccountDTO { UID=a.ShopUID,BonusAmount=a.Amount}).ToArray();
                return result;
            }
        }
    }
}
