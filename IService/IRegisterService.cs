using IMS.DTO;
using System;
using System.Data;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 设置管理接口
    /// </summary>
    public interface IRegisterService : IServiceSupport
    {
        Task<long> AddAsync(RegisterDTO dto);
        Task<RegisterSearchResult> GetRankListWeekAsync();
        Task<RegisterSearchResult> GetRankListTeamAsync();
        Task<RegisterPersonalSearchResult> GetRankListPersonalAsync();
        Task<RegisterResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<bool> Audit(long id);
        Task<bool> Cancel(long id);
        Task<RegisterDTO> GetModelAsync(long id);
        Task<RegisterDTO> GetModelByUserIdAsync(long id);
        bool ImportFromXLS(DataTable dt, out string msg);
        Task<bool> ExistsByTeamNameAsync(string teanName);
        Task<bool> ExistsByContactPhoneAsync(string contactPhone);
        //Task<bool> UpdateAsync(long id, string parm);
        //Task<bool> UpdateAsync(params SettingParm[] parms);
        //Task<bool> DeleteAsync(long id);
        //Task<RegisterDTO> GetModelByNameAsync(string name);
        //Task<RegisterDTO[]> GetModelListAsync(string settingTypeName);
        //Task<RegisterDTO[]> GetModelListAsync(long[] settingTypeIds);
        //Task<SettingSearchResult> GetModelListAsync(long[] settingTypeIds, string keyword,DateTime? startTime,DateTime? endTime,int pageIndex,int pageSize);
    }
    public class RegisterSearchResult
    {
        public RankListWeekModel[] RankList { get; set; }

    }
    public class RegisterPersonalSearchResult
    {
        public RankListPersonalModel[] RankList { get; set; }

    }
    public class RankListWeekModel
    {
        public string TeamName { get; set; }
        public string SchoolName { get; set; }
        public decimal Score { get; set; }

    }
    public class RankListPersonalModel
    {
        public string Name { get; set; }
        public string TeamName { get; set; }
        public string SchoolName { get; set; }
        public decimal Score { get; set; }

    }
    /// <summary>
    /// 获取报名列表

    public class RegisterResult
    {
        public RegisterDTO[] Register { get; set; }
        public long PageCount { get; set; }
    }
}
