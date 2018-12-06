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
    public class SettingTypeService : ISettingTypeService
    {
        private SettingTypeDTO ToDTO(SettingTypeEntity entity)
        {
            SettingTypeDTO dto = new SettingTypeDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.Name = entity.Name;
            dto.Description = entity.Description;
            dto.Sort = entity.Sort;
            dto.IsEnabled = entity.IsEnabled;
            return dto;
        }
        public async Task<long> AddAsync(string name, string description, int sort)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingTypeEntity entity = new SettingTypeEntity();
                entity.Name = name;
                entity.Description = description;
                entity.Sort = sort;
                dbc.SettingTypes.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> EditAsync(long id, string name, string description, int sort)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingTypeEntity entity = await dbc.GetAll<SettingTypeEntity>().SingleOrDefaultAsync(p => p.Id == id);
                if (entity == null)
                {
                    return -1;
                }
                entity.Name = name;
                entity.Description = description;
                entity.Sort = sort;
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> FrozenAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingTypeEntity entity = await dbc.GetAll<SettingTypeEntity>().SingleOrDefaultAsync(p => p.Id == id);
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
                SettingTypeEntity entity = await dbc.GetAll<SettingTypeEntity>().SingleOrDefaultAsync(p => p.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<SettingTypeDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingTypeEntity entity = await dbc.GetAll<SettingTypeEntity>().SingleOrDefaultAsync(p => p.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<SettingTypeDTO[]> GetModelListIsEnableAsync()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<SettingTypeEntity>().AsNoTracking().Where(p => p.IsEnabled == true);
                var idNameTypes = await entities.ToListAsync();
                return idNameTypes.Select(p => ToDTO(p)).ToArray();
            }
        }

        public async Task<SettingTypeDTO[]> GetModelListAsync()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<SettingTypeEntity>().AsNoTracking();
                var idNameTypes = await entities.ToListAsync();
                return idNameTypes.Select(p => ToDTO(p)).ToArray();
            }
        }
    }
}
