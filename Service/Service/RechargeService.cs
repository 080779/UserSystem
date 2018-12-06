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
   public class RechargeService : IRechargeService
    {
        public IIdNameService idNameService { get; set; }
        private RechargeDTO ToDTO(RechargeEntity entity)
        {
            RechargeDTO dto = new RechargeDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.UserID = entity.UserId;
            dto.UserCode = entity.User.Mobile;
            dto.RechargeableMoney = entity.RechargeableMoney;
            dto.YuAmount = entity.YuAmount;
            dto.Flag = entity.Flag;
            dto.RechargeStyle = entity.RechargeStyle;
            dto.RechargeDate = entity.RechargeDate;
            dto.RechargeStyleName = entity.RechargeStyle == 1 ? "A积分" : entity.RechargeStyle == 2 ? "B积分" : entity.RechargeStyle == 3 ? "锁仓积分" :"";
            dto.RechargeTypeName = entity.RechargeType == 1 ? "增加" : "减少";
            dto.Recharge001Name = entity.Recharge001 == 0 ? "后台充值" : "";
            dto.FlagName = entity.Flag == 1 ? "成功" : "失败";

            return dto;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currencyType">积分类型，1-A积分，2-B积分，3-锁仓积分</param>
        /// <param name="rechargeType">充值方式，1-增加，2-扣除</param>
        /// <param name="rechargeableMoney">充值金额</param>
        /// <returns></returns>
        public async Task<long> AddAsync(long userId,int currencyType,int rechargeType,decimal rechargeableMoney)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return -1;
                }
                //if (user.BonusAmount < rechargeableMoney)
                //{
                //    return -2;
                //}
               
                RechargeEntity entity = new RechargeEntity();
                entity.UserId = userId;
                entity.AgentID = 1;
                entity.RechargeStyle = currencyType;
                entity.RechargeType = rechargeType;
                entity.RechargeableMoney = rechargeableMoney;
                entity.Flag = 1;
                entity.RechargeDate = DateTime.Now;
                entity.Recharge001 = 0;
                entity.Recharge002 = 0;
                entity.Recharge005 = 0;
                entity.Recharge006 = 0;

                dbc.Recharges.Add(entity);


                int JournalTypeId = 1;
                JournalEntity journal = new JournalEntity();
                journal.InAmount = rechargeableMoney;
                journal.OutAmount = 0;
                journal.JournalTypeId = JournalTypeId;
                journal.CurrencyType = currencyType;
                journal.Remark = "后台充值" + (currencyType == 1 ? "A积分": currencyType == 2 ? "B积分" :currencyType == 3 ? "锁仓积分":"") + "(增加)";
                journal.UserId = user.Id;
              
                if (rechargeType == 1)
                {
                    if (currencyType == 1)
                    { 
                        entity.YuAmount = user.BonusAmount + rechargeableMoney;
                        journal.BalanceAmount = user.BonusAmount + rechargeableMoney;
                        user.BonusAmount += rechargeableMoney;
                    }
                    else if (currencyType == 2)
                    {
                        entity.YuAmount = user.Amount + rechargeableMoney;
                        journal.BalanceAmount = user.Amount + rechargeableMoney;
                        user.Amount += rechargeableMoney;
                    }
                    else if (currencyType == 3)
                    { 
                        entity.YuAmount = user.FrozenAmount + rechargeableMoney;
                        journal.BalanceAmount = user.FrozenAmount + rechargeableMoney;
                        user.FrozenAmount += rechargeableMoney;
                    }
                }
                else if (rechargeType == 2)
                {
                    journal.InAmount = 0;
                    journal.CurrencyType = currencyType;
                    journal.OutAmount = rechargeableMoney;

                    journal.Remark = "后台充值" + currencyType + "(扣除)";
                    if (currencyType == 1)
                    {
                        if (rechargeableMoney > user.BonusAmount)
                        {
                            return -1;
                        }
                        entity.YuAmount = user.BonusAmount - rechargeableMoney;
                        journal.BalanceAmount = user.BonusAmount - rechargeableMoney;
                        user.BonusAmount -= rechargeableMoney;
                    }
                    else if (currencyType == 2)
                    {
                        if (rechargeableMoney > user.Amount)
                        {
                            return -1;
                        }
                        entity.YuAmount = user.Amount - rechargeableMoney;
                        journal.BalanceAmount = user.Amount - rechargeableMoney;
                        user.Amount -= rechargeableMoney;
                    }

                    else if (currencyType == 3)
                    {
                        if (rechargeableMoney > user.FrozenAmount)
                        {
                            return -1;
                        }
                        entity.YuAmount = user.FrozenAmount - rechargeableMoney;
                        journal.BalanceAmount = user.FrozenAmount - rechargeableMoney;
                        user.FrozenAmount -= rechargeableMoney;
                    }
                }
                
                    dbc.Journals.Add(journal);
             
                   await dbc.SaveChangesAsync();

               
                return entity.Id;
            }
        }
        public async Task<RechargeSearchResult> GetRechargeListAsync(long? userId, string userCode,int rechargtateId, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                RechargeSearchResult result = new RechargeSearchResult();
                var entities = dbc.GetAll<RechargeEntity>();
                if (userId != null)
                {
                    entities = entities.Where(a => a.UserId == userId);
                }
                if (rechargtateId != 0)
                {
                    entities = entities.Where(a => a.RechargeType == rechargtateId);
                }
                if (!string.IsNullOrEmpty(userCode))
                {
                    entities = entities.Where(g => g.User.Mobile.Contains(userCode));
                }
                if (startTime != null)
                {
                    entities = entities.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    entities = entities.Where(a => SqlFunctions.DateDiff("day", endTime, a.CreateTime) <= 0);
                }
                result.PageCount = (int)Math.Ceiling((await entities.LongCountAsync()) * 1.0f / pageSize);
                var transferCashResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Recharges = transferCashResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }
    }
}
