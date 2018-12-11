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
    public class AdminLogService : IAdminLogService
    {
        public AdminLogDTO ToDTO(AdminLogEntity entity)
        {
            AdminLogDTO dto = new AdminLogDTO();
            dto.AdminId = entity.AdminId;
            dto.AdminMobile = entity.AdminMobile;
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.Id = entity.Id;
            dto.IpAddress = entity.IpAddress;
            dto.PermissionTypeId = entity.PermissionTypeId;
            dto.PermissionTypeName = entity.PermissionTypeName;
            dto.Tip = entity.Tip;
            return dto;
        }
        public async Task<long> AddAsync(long adminId, long permissionTypeId, string description, string ipAddress, string tip)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                AdminLogEntity adminLog = new AdminLogEntity();
                adminLog.AdminId = adminId;
                adminLog.PermissionTypeId = permissionTypeId;
                adminLog.Description = description;
                adminLog.IpAddress = ipAddress;
                adminLog.Tip = tip;
                dbc.AdminLogs.Add(adminLog);
                await dbc.SaveChangesAsync();
                return adminLog.Id;
            }
        }
        public long Add(long adminId, string permissionType, string description, string ipAddress, string tip)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                long permissionTypeId = dbc.GetId<PermissionTypeEntity>(p => p.Name == permissionType);
                if(permissionTypeId<=0)
                {
                    return -1;
                }                
                AdminLogEntity adminLog = new AdminLogEntity();
                adminLog.AdminId = adminId;
                adminLog.AdminMobile = dbc.GetParameter<AdminEntity>(a => a.Id == adminId, a => a.Mobile);
                adminLog.PermissionTypeId = permissionTypeId;
                adminLog.PermissionTypeName = permissionType;
                adminLog.Description = description;
                adminLog.IpAddress = ipAddress;
                adminLog.Tip = tip;
                dbc.AdminLogs.Add(adminLog);
                dbc.SaveChanges();
                return adminLog.Id;
            }
        }

        public async Task<AdminLogSearchResult> GetModelListAsync(string keyword, long? permissionTypeId, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                AdminLogSearchResult result = new AdminLogSearchResult();
                var adminLogs = dbc.GetAll<AdminLogEntity>().AsNoTracking();
                if (!string.IsNullOrEmpty(keyword))
                {
                    adminLogs = adminLogs.Where(a => a.AdminMobile.Contains(keyword));
                }
                if (permissionTypeId != null)
                {
                    adminLogs = adminLogs.Where(a => a.PermissionTypeId == permissionTypeId);
                }
                if (startTime != null)
                {
                    adminLogs = adminLogs.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    adminLogs = adminLogs.Where(a => SqlFunctions.DateDiff("day", endTime, a.CreateTime) <= 0);
                }
                result.PageCount = (int)Math.Ceiling((await adminLogs.LongCountAsync()) * 1.0f / pageSize);
                var adminLogsResult = await adminLogs.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.AdminLogs = adminLogsResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }
    }
}
