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
    public class MemberService : IMemberService
    {
        public MemberDTO ToDTO(MemberEntity entity)
        {
            MemberDTO dto = new MemberDTO();
            dto.TeamId = entity.TeamId;
            dto.Name = entity.Name;
            dto.Mobile = entity.Mobile;
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.AddType = entity.AddType;
            dto.BindUserId = entity.BindUserId;
            dto.BindTime = entity.BindTime;
            dto.GRid = entity.GRid;
            return dto;
        }
        public async Task<long> AddAsync(long TeamId, string name, string mobile, int AddType)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                MemberEntity entity = new MemberEntity();
                entity.TeamId = TeamId;
                entity.Name = name;
                entity.Mobile = mobile;
                entity.AddType = AddType;
                dbc.Members.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                MemberEntity entity = await dbc.GetAll<MemberEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<MemberDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<MemberEntity>().AsNoTracking().SingleOrDefaultAsync(a=>a.Id==id);
                if(entity==null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }
        public async Task<MemberDTO> GetModelByMobileAsync(string mobile)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<MemberEntity>().AsNoTracking().FirstOrDefaultAsync(a => a.Mobile == mobile);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<MemberSearchResult> GetModelListAsync(long? teamId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                MemberSearchResult result = new MemberSearchResult();
                var entities = dbc.GetAll<MemberEntity>().AsNoTracking();
                if (teamId != null)
                {
                    entities = entities.Where(a => a.TeamId == teamId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Name.Contains(keyword) || g.Mobile.Contains(keyword));
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
                var memberResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Member = memberResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }       

        public async Task<bool> UpdateAsync(long id, string name, string mobile)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                MemberEntity entity = await dbc.GetAll<MemberEntity>().SingleOrDefaultAsync(a=>a.Id==id);
                if(entity==null)
                {
                    return false;
                }
                entity.Name = name;
                entity.Mobile = mobile;
                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
