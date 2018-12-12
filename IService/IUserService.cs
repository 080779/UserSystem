using IMS.DTO;
using System;
using System.Data;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface IUserService:IServiceSupport
    {
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="mobile">账号</param>
        /// <param name="password">密码</param>
        /// <param name="levelTypeId">等级id</param>
        /// <returns></returns>
        Task<long> AddAsync(string mobile, int levelTypeId, string password, string tradePassword, string recommend, string nickName, string avatarUrl);
        Task<long> AddByExcelAsync(string mobile, string trueName, int levelTypeId, string password, string tradePassword, string recommend, string nickName, string avatarUrl);
        Task<bool> AddAmountAsync(string mobile,decimal amount);
        Task<bool> AddAntegralAsync(long id,int integral);
        /// <summary>
        /// 修改头像，昵称
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="nickName">要修改的昵称（为null不修改）</param>
        /// <param name="headpic">要修改的头像地址（为null不修改）</param>
        /// <returns></returns>
        Task<bool> UpdateInfoAsync(long id,string nickName, string headpic, string trueName);
        Task<bool> UpdateShareCodeAsync(long id, string codeUrl);
        Task<bool> UpdateTeamShareCodeAsync(long id, string codeUrl);
        Task<bool> UpdatePhone(long id, string phone);
        /// <summary>
        /// 激活会员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> ActivateAsync(long id);
        /// <summary>
        /// 批量激活会员
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<bool> ActivateAllAsync(long[] ids);
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(long id);
        /// <summary>
        /// 冻结用户
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns></returns>
        Task<bool> FrozenAsync(long id);
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        Task<long> ResetPasswordAsync(long id, string password, string newPassword);
        Task<long> ResetPasswordAsync(long id, string password);
        Task<long> ResetPasswordAsync(string mobile, string password);
        Task<long> ResetTradePasswordAsync(string mobile, string password);
        Task<long> UserCheck(string mobile);
        Task<long> CheckLoginAsync(string mobile,string password);
        Task<long> CheckTradePasswordAsync(long id,string tradePassword);
        bool CheckUserId(long id);
        Task<long> BalancePayAsync(long orderId);
        long WeChatPay(string code);
        Task<CalcAmountResult> CalcCount();
        UserDTO GetModel(long id);
        /// <summary>
        /// 根据id获得用户模型
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns></returns>
        Task<UserDTO> GetModelAsync(long id);
        Task<long> GetUserRecommendIdAysnc(long userId);
        Task<UserRecommendTreeDTO> GetModelTreeAsync(long id);
        Task<string> GetMobileByIdAsync(long id);
        Task<string> GetUserCodeByIdAsync(long id);
        /// <summary>
        /// 根据用户账号获得用户模型
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        Task<UserDTO> GetModelByMobileAsync(string mobile);
        Task<UserRecommendTreeDTO> GetModelTreeByMobileAsync(string mobile);
        Task<UserDTO> GetModelByUserCodeAsync(string userCode);
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="levelId">用户等级id</param>
        /// <param name="keyword">搜索关键字</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        Task<UserSearchResult> GetModelListAsync(int? levelId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        long GetUserShopUIDAsync(string mobile);
        Task<long> GetIdByMobile(string mobile);

        Task<int> AccountChange(string mobile, int type, decimal amount, string remark);
        /// <summary>
        /// 累计业绩
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sroce"></param>
        /// <returns></returns>
        Task<bool> CalcSroce(long userId, decimal sroce);
        Task<UserRecommendResult> GetFirstRecommendListAsync(string mobile, int pageIndex, int pageSize);
        Task<bool> ManageLevelUp();
        Task<long> SetLevelUpAsync(string mobile, int levelId);
        Task<int> SetMLevelUpAsync(string mobile, int levelId);
        Task<string> SearchMlevelNameAsync(string mobile);
        Task<UserRecommendTreeDTO[]> GetRecommendListAsync(long id);
        Task<UserSearchResult> GetActivateListAsync(int pageIndex, int pageSize);
    }
    public class UserSearchResult
    {
        public UserDTO[] Users { get; set; }
        public long PageCount { get; set; }
    }
    public class UserTeamSearchResult
    {
        public UserDTO TeamLeader { get; set; }
        public UserDTO[] Members { get; set; }
        public long PageCount { get; set; }
        public long TotalCount { get; set; }
    }
    public class CalcAmountResult
    {
        public decimal TotalAmount { get; set; }
        public decimal TotalTakeCash { get; set; }
        public decimal TotalBuyAmount { get; set; }
    }

    public class UserRecommendResult
    {
        public UserRecommendDTO[] UserRecommends { get; set; }
        public long PageCount { get; set; }
    }
}
