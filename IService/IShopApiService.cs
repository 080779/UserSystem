using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 接口
    /// </summary>
    public interface IShopApiService : IServiceSupport
    {
        Task<decimal> GetBalanceAsync(long uid);
        Task<bool> SetBalanceAsync(long uid, int type, decimal num, int from_type);
        Task<bool> CheckTradePassAsync(long uid, string pass);
    }

}
