using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IMS.IService;
using IMS.DTO;

namespace IMS.Service.Service
{
    public class LinkService : ILinkService
    {        
        private LinkDTO ToDTO(LinkEntity entity)
        {
            LinkDTO dto = new LinkDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.Name = entity.Name;
            dto.ImgUrl = entity.ImgUrl;
            dto.Url = entity.Url;
            dto.Tip = entity.Tip;
            dto.TypeId = entity.TypeId;
            dto.Sort = entity.Sort;
            dto.IsEnabled = entity.IsEnabled;
            dto.Link001 = entity.Link001;
            dto.link002 = entity.link002;
            dto.TypeName = entity.TypeName;
            return dto;
        }

        public async Task<long> AddAsync(long typeId, string name, string imgUrl, string url, int sort)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                LinkEntity entity = new LinkEntity();
                entity.Name = name;
                entity.ImgUrl = imgUrl;
                entity.Url = url;
                entity.Sort = sort;
                entity.TypeId = typeId;
                dbc.Links.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> AddAsync(long typeId, string typeName, string name, string imgUrl, string url, decimal amount, int integral)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                LinkEntity entity = new LinkEntity();
                entity.Name = name;
                entity.ImgUrl = imgUrl;
                entity.Url = url;
                entity.TypeId = typeId;
                entity.TypeName = typeName;
                entity.Link001 = amount;
                entity.link002 = integral;
                dbc.Links.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> EditAsync(long id, string name, string imgUrl, string url, int sort)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                LinkEntity entity = await dbc.GetAll<LinkEntity>().SingleOrDefaultAsync(p => p.Id == id);
                if (entity == null)
                {
                    return -1;
                }
                entity.Name = name;
                entity.ImgUrl = imgUrl;
                entity.Url = url;
                entity.Sort = sort;
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> EditAsync(long id, string name, decimal amount, int integral)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                LinkEntity entity = await dbc.GetAll<LinkEntity>().SingleOrDefaultAsync(p => p.Id == id);
                if (entity == null)
                {
                    return -1;
                }
                entity.Name = name;
                entity.Link001 = amount;
                entity.link002 = integral;
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> FrozenAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                LinkEntity entity = await dbc.GetAll<LinkEntity>().SingleOrDefaultAsync(p => p.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsEnabled = !entity.IsEnabled;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                LinkEntity entity = await dbc.GetAll<LinkEntity>().SingleOrDefaultAsync(p => p.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<LinkDTO> GetModelByIdAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                LinkEntity entity = await dbc.GetAll<LinkEntity>().SingleOrDefaultAsync(p => p.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<LinkDTO[]> GetByTypeIdIsEnableAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<LinkEntity>().AsNoTracking().Where(p => p.TypeId == id && p.IsEnabled == true);
                var idNames = await entities.OrderBy(p => p.Sort).ToListAsync();
                return idNames.Select(p => ToDTO(p)).ToArray();
            }
        }

        public async Task<LinkDTO[]> GetByTypeNameIsEnableAsync(string name)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<LinkEntity>().AsNoTracking().Where(p => p.TypeName == name && p.IsEnabled == true);
                var idNames = await entities.OrderBy(p => p.Sort).ToListAsync();
                return idNames.Select(p => ToDTO(p)).ToArray();
            }
        }

        public async Task<LinkDTO[]> GetByTypeIdAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<LinkEntity>().AsNoTracking().Where(p => p.TypeId == id);
                var idNames = await entities.OrderBy(p => p.Sort).ToListAsync();
                return idNames.Select(p => ToDTO(p)).ToArray();
            }
        }

        public async Task<LinkSearchResult> GetModelListAsync(long typeId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                LinkSearchResult result = new LinkSearchResult();
                var links = dbc.GetAll<LinkEntity>().AsNoTracking().Where(l => l.TypeId == typeId);
                if (!string.IsNullOrEmpty(keyword))
                {
                    links = links.Where(a => a.Name.Contains(keyword));
                }
                if (startTime != null)
                {
                    links = links.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    links = links.Where(a => a.CreateTime.Year <= endTime.Value.Year && a.CreateTime.Month <= endTime.Value.Month && a.CreateTime.Day <= endTime.Value.Day);
                }
                result.PageCount = (int)Math.Ceiling((await links.LongCountAsync()) * 1.0f / pageSize);
                var linksResult = await links.OrderBy(a => a.Sort).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Links = linksResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<LinkSearchResult> GetModelListAsync(string typeName, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                LinkSearchResult result = new LinkSearchResult();
                var links = dbc.GetAll<LinkEntity>().AsNoTracking().Where(l => l.TypeName == typeName);
                if (!string.IsNullOrEmpty(keyword))
                {
                    links = links.Where(a => a.Name.Contains(keyword));
                }
                if (startTime != null)
                {
                    links = links.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    links = links.Where(a => a.CreateTime.Year <= endTime.Value.Year && a.CreateTime.Month <= endTime.Value.Month && a.CreateTime.Day <= endTime.Value.Day);
                }
                result.PageCount = (int)Math.Ceiling((await links.LongCountAsync()) * 1.0f / pageSize);
                var linksResult = await links.OrderBy(a => a.Sort).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Links = linksResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }
    }
}
