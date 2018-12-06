using IMS.Common.Enums;
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
    public class CashSellService : ICashSellService
    {
        public IIdNameService idNameService { get; set; }
        public IJournalService journalService { get; set; }
        public IShopApiService shopApiService { get; set; }
        public CashSellDTO ToDTO(CashSellEntity entity)
        {
            CashSellDTO dto = new CashSellDTO();
            dto.UserId = entity.UserId;
            dto.SellerUserCode = entity.Seller.UserCode;
            dto.SellerUserName = entity.Seller.NickName;
            dto.OrderCode = entity.OrderCode;
            dto.Number = entity.Number;
            dto.Price = entity.Price;
            dto.Amount = entity.Amount;
            dto.Charge = entity.Charge;
            dto.BalanceNum = entity.BalanceNum;
            dto.StateType = entity.StateType;
            dto.StateName = entity.StateType.GetEnumName<CashSellStateEnums>();
            dto.SellType = entity.SellType;
            dto.CurrencyType = entity.CurrencyType;
            //dto.CurrencyName = CurrencyHelper.GetName(entity.CurrencyType);
            dto.CurrencyName = entity.CurrencyType.GetEnumName<CurrencyEnums>();
            dto.CreateTime = entity.CreateTime;
            return dto;
        }
        public async Task<long> AddAsync(long userId, string orderCode, int number, decimal price, decimal amount, decimal charge, int balanceNum, int accountType)
        {
            decimal balanceAmount = 0;
            using (MyDbContext dbc = new MyDbContext())
            {
                var userSellEntity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(w => w.Id == userId);
                if (userSellEntity == null)
                    return -1;

                if (accountType == 1) //A积分
                {
                    if (userSellEntity.BonusAmount < number)
                        return -2;//账户余额不足
                }
                else //B积分
                {
                    if (userSellEntity.Amount < number)
                        return -2;//账户余额不足
                }
                using (var scope = dbc.Database.BeginTransaction())
                {
                    try
                    {
                        CashSellEntity sellEntity = new CashSellEntity();
                        sellEntity.UserId = userId;
                        sellEntity.OrderCode = orderCode;
                        sellEntity.Number = number;
                        sellEntity.Price = price;
                        sellEntity.Amount = amount;
                        sellEntity.Charge = charge;
                        sellEntity.BalanceNum = balanceNum;
                        sellEntity.StateType = 0;
                        sellEntity.SellType = 1; //SellType 0:卖出，1：挂卖
                        sellEntity.CurrencyType = accountType;
                        dbc.CashSells.Add(sellEntity);

                        //卖方 扣除积分
                        #region 卖方 扣除积分

                        if (accountType == 1)//A积分
                        {
                            userSellEntity.BonusAmount -= number;
                            balanceAmount = userSellEntity.BonusAmount;
                        }
                        else if (accountType == 2)//B积分
                        {
                            userSellEntity.Amount -= number;
                            balanceAmount = userSellEntity.Amount;
                        }
                        else
                        {
                            scope.Rollback();
                            return -4;
                        }
                        int JournalTypeId = (int)JournalTypeEnum.交易卖出;
                        await journalService.AddAsync(userSellEntity.Id, number, JournalTypeId, sellEntity.CurrencyType, balanceAmount, "交易卖出", 0);
                        #endregion
                        await dbc.SaveChangesAsync();
                        scope.Commit();
                        return sellEntity.Id;
                    }
                    catch (Exception ex)
                    {
                        scope.Rollback();
                        return -3;
                    }
                }
                
               
            }
        }
        /// <summary>
        /// 出售给买方
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="buyId"></param>
        /// <param name="sellNum"></param>
        /// <returns></returns>
        public async Task<long> SellToAsync(long userId,long buyId, int sellNum  )
        {
            decimal balanceAmount = 0 , payAmount = 0;
            using (MyDbContext dbc = new MyDbContext())
            {

                var buyEntity = await dbc.GetAll<CashBuyEntity>().SingleOrDefaultAsync(w=>w.Id == buyId);
                if (buyEntity == null)
                    return -1;

                if (buyEntity.Buyer.Id == userId)
                    return -8; //不能出售给自己挂买的订单

                if (buyEntity.BalanceNum < sellNum)
                    return -2;//出售数量不能大于交易数量

                using (var scope = dbc.Database.BeginTransaction())
                {
                    try
                    {
                        string orderCode = Common.CommonHelper.CreateNo();
                        CashSellEntity sell = new CashSellEntity();
                        sell.UserId = userId;
                        sell.OrderCode = orderCode;
                        sell.Number = sellNum;
                        sell.Price = buyEntity.Price;
                        sell.Amount = sellNum * buyEntity.Price;
                        sell.Charge = 0;
                        sell.BalanceNum = 0;
                        sell.StateType = 1;
                        sell.SellType = 0; //SellType 0:卖出，1：挂卖
                        sell.CurrencyType = buyEntity.CurrencyType;
                        dbc.CashSells.Add(sell);
                        await dbc.SaveChangesAsync();

                        CashOrderEntity order = new CashOrderEntity();
                        order.BuyId = buyId;
                        order.SellId = sell.Id;
                        order.BuyUserId = buyEntity.UserId;
                        order.SellUserId = userId;
                        order.OrderCode = orderCode;
                        order.Number = sellNum;
                        order.Price = buyEntity.Price;
                        order.Amount = sellNum * buyEntity.Price;
                        order.PayStateType = 1;
                        order.PayTime = DateTime.Now;
                        order.ConfirmStateType = 1;
                        order.ConfirmTime = DateTime.Now;
                        order.StateType = 1;
                        dbc.CashOrders.Add(order);

                        buyEntity.BalanceNum -= sellNum;
                        if (buyEntity.BalanceNum == 0)
                            buyEntity.StateType = 1;

                        //卖方 扣除积分
                        var userSellEntity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(w => w.Id == userId);

                        if (buyEntity.CurrencyType == 1)//A积分
                        {
                            userSellEntity.BonusAmount -= sellNum;
                            balanceAmount = userSellEntity.BonusAmount;
                        }
                        else if (buyEntity.CurrencyType == 2)//B积分
                        {
                            userSellEntity.Amount -= sellNum;
                            balanceAmount = userSellEntity.Amount;
                        }
                        else
                        {
                            scope.Rollback();
                            return -4;
                        }
                        int JournalTypeId = (int)JournalTypeEnum.交易卖出;
                        await journalService.AddAsync(userSellEntity.Id, sellNum, JournalTypeId, buyEntity.CurrencyType, balanceAmount, "交易卖出", 0);

                        //买方 增加积分
                        var userBuyEntity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(w => w.Id == buyEntity.UserId);

                        if (buyEntity.CurrencyType == 1)//A积分
                            userBuyEntity.BonusAmount += sellNum;
                        else if (buyEntity.CurrencyType == 2)//B积分
                            userBuyEntity.Amount += sellNum;
                        else
                        {
                            scope.Rollback();
                            return -4;
                        }
                        JournalTypeId = (int)JournalTypeEnum.交易买入;
                        await journalService.AddAsync(userId, sellNum, JournalTypeId, buyEntity.CurrencyType, balanceAmount, "交易买入", 1);

                        //卖方增加账户余额
                        //修改商城账户余额,type 0为减少，1为增加
                        payAmount = buyEntity.Price * sellNum;
                        var getResult = await shopApiService.SetBalanceAsync(userSellEntity.ShopUID, 1, payAmount, buyEntity.CurrencyType);
                        if (!getResult)
                            return -7;//卖方账户增加出错

                        await dbc.SaveChangesAsync();
                        scope.Commit();
                        return sell.Id;
                    }
                    catch (Exception ex)
                    {
                        scope.Rollback();
                        return -3;
                    }
                }
            }
        }

        //public async Task<long> AddJournal()
        //{

        //}

        public async Task<CashSellSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                CashSellSearchResult result = new CashSellSearchResult();
                var logs = dbc.GetAll<CashSellEntity>().AsNoTracking();
                if (!string.IsNullOrEmpty(keyword))
                {
                    logs = logs.Where(a => a.OrderCode.Contains(keyword) );
                }
               
                if (startTime != null)
                {
                    logs = logs.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    logs = logs.Where(a => SqlFunctions.DateDiff("day", endTime, a.CreateTime) <= 0);
                }
                result.TotalCount = await logs.LongCountAsync();
                result.PageCount = (int)Math.Ceiling((result.TotalCount) * 1.0f / pageSize);
                var logsResult = await logs.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.log = logsResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }
        //出售订单
        public async Task<CashSellSearchResult> SellListAsync(long? userId, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                CashSellSearchResult result = new CashSellSearchResult();
                var logs = dbc.GetAll<CashSellEntity>().AsNoTracking();
                if (userId != null && userId> 0)
                {
                    logs = logs.Where(a => a.UserId == userId);
                }
                result.TotalCount = await logs.LongCountAsync();
                result.PageCount = (int)Math.Ceiling((result.TotalCount) * 1.0f / pageSize);
                var logsResult = await logs.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.log = logsResult.Select(a => ToDTO(a)).ToArray();
                
                return result;
            }
        }
       
    }
}
