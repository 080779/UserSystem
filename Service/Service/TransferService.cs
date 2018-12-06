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
    public class TransferService : ITransferService
    {
        public IIdNameService idNameService { get; set; }
        private TransferADTO ToDTO(TransferEntity entity)
        {
            TransferADTO dto = new TransferADTO();
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.Amount = entity.Amount;
            dto.ChangeTypeName = entity.ChangeType.ToString() == "1" ? "A积分转换B积分": "";
          
            return dto;
        }
        public async Task<long> AddAsync(long userId, decimal amount)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return -1;
                }
                if (user.BonusAmount < amount)
                {
                    return -2;
                }

                TransferEntity entity = new TransferEntity();
                entity.UserId = userId;
                entity.ChangeType = 1;
                entity.Amount = amount;
                dbc.Transfer.Add(entity);

                user.BonusAmount -= amount;
                user.Amount += amount;

                int JournalTypeId = 1;
                JournalEntity journal = new JournalEntity();
                journal.OutAmount = amount;
                journal.InAmount = 0;
                journal.JournalTypeId = JournalTypeId;
                journal.Remark = "A积分转换B积分";
                journal.UserId = userId;
                journal.CurrencyType = 1;
                journal.BalanceAmount = user.BonusAmount;
                dbc.Journals.Add(journal);

                JournalEntity journalInfo = new JournalEntity();
                journalInfo.OutAmount = 0;
                journalInfo.InAmount = amount;
                journalInfo.JournalTypeId = JournalTypeId;
                journalInfo.Remark = "A积分转换B积分";
                journalInfo.UserId = userId;
                journalInfo.CurrencyType = 2;
                journalInfo.BalanceAmount = user.Amount;
                dbc.Journals.Add(journalInfo);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }
        public async Task<TransferACashSearchResult> GetTransferListAsync(long? userId, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                TransferACashSearchResult result = new TransferACashSearchResult();
                var entities = dbc.GetAll<TransferEntity>();
                if (userId != null)
                {
                    entities = entities.Where(a => a.UserId == userId);
                }
               
                result.PageCount = (int)Math.Ceiling((await entities.LongCountAsync()) * 1.0f / pageSize);
                var transferCashResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.TakeCashesA = transferCashResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }
    }
}
