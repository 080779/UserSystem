using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 参数类型接口
    /// </summary>
    public interface ISettingTypeService : IServiceSupport
    {
        Task<long> AddAsync(string name, string description, int sort);
        Task<long> EditAsync(long id, string name, string description, int sort);
        Task<bool> FrozenAsync(long id);
        Task<bool> DelAsync(long id);
        Task<SettingTypeDTO> GetModelAsync(long id);
        Task<SettingTypeDTO[]> GetModelListIsEnableAsync();
        Task<SettingTypeDTO[]> GetModelListAsync();
    }
}
