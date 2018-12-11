using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 图片链接接口
    /// </summary>
    public interface ILinkService : IServiceSupport
    {
        Task<long> AddAsync(long typeId, string name, string imgUrl, string url, int sort);
        Task<long> AddAsync(long typeId,string typeName, string name, string imgUrl, string url,decimal amount,int integral);
        Task<long> EditAsync(long id, string name, string imgUrl, string url, int sort);
        Task<long> EditAsync(long id, string name, decimal amount, int integral);
        Task<bool> FrozenAsync(long id);
        Task<bool> DelAsync(long id);
        Task<LinkDTO> GetModelByIdAsync(long id);
        Task<LinkDTO[]> GetByTypeIdIsEnableAsync(long id);
        Task<LinkDTO[]> GetByTypeNameIsEnableAsync(string name);
        Task<LinkDTO[]> GetByTypeIdAsync(long id);
        Task<LinkSearchResult> GetModelListAsync(long typeId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<LinkSearchResult> GetModelListAsync(string typeName, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class LinkSearchResult
    {
        public LinkDTO[] Links { get; set; }
        public long PageCount { get; set; }
    }
}
