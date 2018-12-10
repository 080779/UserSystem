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
            dto.Remark = entity.Remark;
            dto.ParamTypeId = entity.ParamTypeId;
            dto.Param = entity.Param;
            dto.Sort = entity.Sort;
            dto.IsEnabled = entity.IsEnabled;
            dto.Sort = entity.Sort;
            dto.ParamName = entity.ParamName;
            dto.LevelId = entity.LevelId;
            return dto;
        }

        public async Task<long> AddAsync(string name, string param, string remark, int sort, int typeId)
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
                entity.Param = param;
                entity.Sort = sort;
                entity.ParamTypeId = typeId;
                entity.Remark = remark;
                dbc.Settings.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> EditAsync(long id, string name, string param, string remark, int sort)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingEntity entity = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(p => p.Id == id);
                if (entity == null)
                {
                    return -1;
                }
                entity.Name = name;
                entity.Param = param;
                entity.Sort = sort;
                entity.Remark = remark;
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

        public async Task<SettingDTO[]> GetByTypeIdIsEnableAsync(int id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<SettingEntity>().AsNoTracking().Where(p => p.ParamTypeId == id && p.IsEnabled == true);
                var idNames = await entities.OrderBy(p => p.Sort).ToListAsync();
                return idNames.Select(p => ToDTO(p)).ToArray();
            }
        }

        public async Task<SettingDTO[]> GetByTypeIdAsync(int id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<SettingEntity>().AsNoTracking().Where(p => p.ParamTypeId == id);
                var idNames = await entities.OrderBy(p => p.Sort).ToListAsync();
                return idNames.Select(p => ToDTO(p)).ToArray();
            }
        }

        public async Task<SettingDTO> GetModelByNameAsync(string name)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingEntity entity = await dbc.GetAll<SettingEntity>().AsNoTracking().SingleOrDefaultAsync(g => g.Name == name);
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
                string parm = await dbc.GetParameterAsync<SettingEntity>(g => g.Name == name,g=>g.Param);
                if (parm == null)
                {
                    return null;
                }
                return parm;
            }
        }

        public async Task<SettingDTO[]> GetModelListAsync(string paramName,int? typeId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<SettingEntity>().AsNoTracking();
                if(!string.IsNullOrEmpty(paramName))
                {
                    entities = entities.Where(s => s.ParamName == paramName);
                }
                if (typeId != null)
                {
                    entities = entities.Where(s => s.ParamTypeId == typeId);
                }
                var settingsResult = await entities.ToListAsync();
                return settingsResult.Select(a => ToDTO(a)).ToArray();
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
                entity.Param = parm;
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
                    entity.Param = parm.Parm.ToString();
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
                entity.Param = parm;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<SettingDTO[]> GetAllIsEnableAsync()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<SettingEntity>().AsNoTracking().Where(s=>s.IsEnabled==true);
                var settingsResult = await entities.ToListAsync();
                return settingsResult.Select(a => ToDTO(a)).ToArray();
            }
        }
    }
}
