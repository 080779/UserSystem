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
    public class SettingService : ISettingService
    {
        private SettingDTO ToDTO(SettingEntity entity)
        {
            SettingDTO dto = new SettingDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.Name = entity.Name;
            dto.Description = entity.Description;
            dto.TypeId = entity.SettingTypeId;
            dto.Parm = entity.Parm;
            dto.Sort = entity.Sort;
            dto.IsEnabled = entity.IsEnabled;
            return dto;
        }

        public PHPSettingDTO PToDTO(SettingEntity entity)
        {
            PHPSettingDTO dto = new PHPSettingDTO();
            dto.Id = entity.Id;
            dto.Description = entity.Description;
            dto.Name = entity.Name;
            dto.Parm = entity.Parm;
            return dto;
        }

        private SettingSetDTO ToDTO(string typeName, SettingDTO[] settings)
        {
            SettingSetDTO dto = new SettingSetDTO();
            dto.TypeName = typeName;
            dto.Settings = settings;
            return dto;
        }

        public async Task<long> AddAsync(string name, string parm, string description, int sort, long typeId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                string typeName = await dbc.GetParameterAsync<SettingTypeEntity>(p => p.Id == typeId, p => p.Name);
                if (string.IsNullOrEmpty(typeName))
                {
                    return -1;
                }
                SettingEntity entity = new SettingEntity();
                entity.Name = name;
                entity.Parm = parm;
                entity.Sort = sort;
                entity.SettingTypeId = typeId;
                entity.Description = description;
                dbc.Settings.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> EditAsync(long id, string name, string parm, string description, int sort)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingEntity entity = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(p => p.Id == id);
                if (entity == null)
                {
                    return -1;
                }
                entity.Name = name;
                entity.Parm = parm;
                entity.Sort = sort;
                entity.Description = description;
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> FrozenAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingEntity entity = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(p => p.Id == id);
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
                SettingEntity entity = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(p => p.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<SettingDTO> GetModelByIdAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<SettingEntity>().AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<SettingDTO[]> GetByTypeIdIsEnableAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<SettingEntity>().AsNoTracking().Where(p => p.SettingTypeId == id && p.IsEnabled == true);
                var idNames = await entities.OrderBy(p => p.Sort).ToListAsync();
                return idNames.Select(p => ToDTO(p)).ToArray();
            }
        }

        public async Task<SettingDTO[]> GetByTypeIdAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<SettingEntity>().AsNoTracking().Where(p => p.SettingTypeId == id);
                var idNames = await entities.OrderBy(p => p.Sort).ToListAsync();
                return idNames.Select(p => ToDTO(p)).ToArray();
            }
        }

        public async Task<SettingDTO> GetModelByNameAsync(string name)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingEntity entity = await dbc.GetAll<SettingEntity>().Include(s => s.SettingType).AsNoTracking().SingleOrDefaultAsync(g => g.Name == name);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<string> GetParmByNameAsync(string name)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                string parm = await dbc.GetParameterAsync<SettingEntity>(g => g.Name == name,g=>g.Parm);
                if (parm == null)
                {
                    return null;
                }
                return parm;
            }
        }

        public async Task<PHPSettingDTO[]> GetModelListAsync(string settingTypeName)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                long settingTypeId = await dbc.GetIdAsync<SettingTypeEntity>(i => i.Name == settingTypeName);
                var entities = dbc.GetAll<SettingEntity>().Include(s => s.SettingType).AsNoTracking().Where(a=>a.SettingTypeId==settingTypeId && a.IsEnabled==true);
                var settingsResult = await entities.ToListAsync();
                return settingsResult.Select(a => PToDTO(a)).ToArray();
            }
        }

        public async Task<bool> UpdateAsync(long id, string parm)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingEntity entity = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.Parm = parm;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateAsync(params SettingParm[] parms)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                foreach(SettingParm parm in parms)
                {
                    SettingEntity entity = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(g => g.Id == parm.Id);
                    if (entity == null)
                    {
                        return false;
                    }
                    entity.Parm = parm.Parm.ToString();
                }
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateByNameAsync(string name, string parm)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingEntity entity = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(g => g.Name == name);
                if (entity == null)
                {
                    return false;
                }
                entity.Parm = parm;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<SettingSetDTO[]> GetAllIsEnableAsync()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<SettingTypeEntity>().AsNoTracking().Where(p => p.IsEnabled == true);
                var parameterSettings = await entities.OrderBy(p => p.Sort).ToListAsync();
                return parameterSettings.Select(p => ToDTO(p.Name, dbc.GetAll<SettingEntity>().AsNoTracking().Where(pp => pp.SettingTypeId == p.Id && pp.IsEnabled==true).ToList().Select(pp => ToDTO(pp)).ToArray())).ToArray();
            }
        }
    }
}
