using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace IMS.IService
{
  public interface ITransferService : IServiceSupport
    {
        Task<long> AddAsync(long userId, decimal amount);
        Task<TransferACashSearchResult> GetTransferListAsync(long? userId, int pageIndex, int pageSize);
    }
    public class TransferCashSearchResult
    {
        public TransferDTO[] TakeCashes { get; set; }
        public long PageCount { get; set; }
    }

  public class TransferACashSearchResult
   {
    public TransferADTO[] TakeCashesA { get; set; }
    public long PageCount { get; set; }
}
}

