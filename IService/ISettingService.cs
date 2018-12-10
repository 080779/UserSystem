using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 设置管理接口
    /// </summary>
    public interface ISettingService : IServiceSupport
    {
        Task<long> AddAsync(string name, string param, string remark, int sort, int typeId);
        Task<long> EditAsync(long id, string name, string param, string remark, int sort);
        Task<bool> FrozenAsync(long id);
        Task<bool> DelAsync(long id);
        Task<SettingDTO> GetModelByIdAsync(long id);
        Task<SettingDTO[]> GetByTypeIdIsEnableAsync(int id);
        Task<SettingDTO[]> GetByTypeIdAsync(int id);
        Task<SettingDTO[]> GetAllIsEnableAsync();
        Task<bool> UpdateAsync(long id, string parm);
        Task<bool> UpdateAsync(params SettingParm[] parms);
        Task<bool> UpdateByNameAsync(string name, string parm);
        Task<SettingDTO> GetModelByNameAsync(string name);
        Task<string> GetParmByNameAsync(string name);
        Task<SettingDTO[]> GetModelListAsync(string paramName, int? typeId);
    }
    public class SettingSearchResult
    {
        public SettingDTO[] Settings { get; set; }
        public long PageCount { get; set; }
    }
    public class SettingParm
    {
        public long Id { get; set; }
        public string Parm { get; set; }
    }
}
