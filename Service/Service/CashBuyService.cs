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
    public class CashBuyService : ICashBuyService
    {
        public IJournalService journalService { get; set; }
        public IIdNameService idNameService { get; set; }
        public IShopApiService shopApiService { get; set; }
        public CashBuyDTO ToDTO(CashBuyEntity entity)
        {
            CashBuyDTO dto = new CashBuyDTO();
            dto.UserId = entity.UserId;
            dto.BuyerUserCode = entity.Buyer.UserCode;
            dto.BuyerUserName = entity.Buyer.NickName;
            dto.OrderCode = entity.OrderCode;
            dto.Number = entity.Number;
            dto.Price = entity.Price;
            dto.Amount = entity.Amount;
            dto.Charge = entity.Charge;
            dto.BalanceNum = entity.BalanceNum;
            dto.StateType = entity.StateType;
            dto.StateName = entity.StateType.GetEnumName<CashBuyStateEnums>();
            dto.BuyType = entity.BuyType;
            dto.CurrencyType = entity.CurrencyType;
            dto.CurrencyName = entity.CurrencyType.GetEnumName<CurrencyEnums>();
            dto.CreateTime = entity.CreateTime;
            return dto;
        }
        public async Task<long> BuyFromAsync(long userId,long buyerUid, long sellId, int buyNum)
        {
            decimal balanceAmount = 0,payAmount = 0 ;
            using (MyDbContext dbc = new MyDbContext())
            {

                var sellEntity = await dbc.GetAll<CashSellEntity>().SingleOrDefaultAsync(w => w.Id == sellId);
                if (sellEntity == null)
                    return -1;

                if (sellEntity.Seller.Id == userId)
                    return -8; //不能买入自己挂卖的订单

                if (sellEntity.BalanceNum < buyNum)
                    return -2;//买入数量不能大于交易数量

                payAmount = sellEntity.Price * buyNum;
                //获取商城账户余额
                var shopBalance = await shopApiService.GetBalanceAsync(buyerUid);
                if(shopBalance < payAmount)
                    return -5;//账户余额不足

                using (var scope = dbc.Database.BeginTransaction())
                {
                    try
                    {
                        #region 买入订单、交易订单
                        string orderCode = Common.CommonHelper.CreateNo();
                        CashBuyEntity buy = new CashBuyEntity();
                        buy.UserId = userId;
                        buy.OrderCode = orderCode;
                        buy.Number = buyNum;
                        buy.Price = sellEntity.Price;
                        buy.Amount = buyNum * sellEntity.Price;
                        buy.Charge = 0;
                        buy.BalanceNum = buyNum;
                        buy.StateType = 1;
                        buy.BuyType = 0;//BuyType 0:买入，1：求购
                        buy.CurrencyType = sellEntity.CurrencyType;
                        dbc.CashBuys.Add(buy);
                        await dbc.SaveChangesAsync();

                        CashOrderEntity order = new CashOrderEntity();
                        order.BuyId = buy.Id;
                        order.SellId = sellId;
                        order.BuyUserId = userId;
                        order.SellUserId = sellEntity.UserId;
                        order.OrderCode = orderCode;
                        order.Number = buyNum;
                        order.Price = sellEntity.Price;
                        order.Amount = buyNum * sellEntity.Price;
                        order.PayStateType = 1;
                        order.PayTime = DateTime.Now;
                        order.ConfirmStateType = 1;
                        order.ConfirmTime = DateTime.Now;
                        order.StateType = 1;
                        dbc.CashOrders.Add(order);

                        sellEntity.BalanceNum -= buyNum;
                        if (sellEntity.BalanceNum == 0)
                            sellEntity.StateType = (int)CashSellStateEnums.已完成;
                        #endregion


                        //买方 增加积分
                        #region 买方 增加积分
                        var userBuyEntity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(w => w.Id == userId);
                        var userSellEntity = await dbc.GetAll<UserEntity>().AsNoTracking().SingleOrDefaultAsync(w => w.Id == sellEntity.UserId);

                        if (sellEntity.CurrencyType == 1)//A积分
                        {
                            userBuyEntity.BonusAmount += buyNum;
                            balanceAmount = userBuyEntity.BonusAmount;
                        }
                        else if (sellEntity.CurrencyType == 2)//B积分
                        {
                            userBuyEntity.Amount += buyNum;
                            balanceAmount = userBuyEntity.Amount;
                        }
                        else
                        {
                            scope.Rollback();
                            return -4;
                        }
                        int JournalTypeId = 1;
                        await journalService.AddAsync(userId, buyNum, JournalTypeId, sellEntity.CurrencyType, balanceAmount, "交易买入", 1);
                        #endregion
                        
                        await dbc.SaveChangesAsync();

                        //买方扣除账户余额
                        //修改商城账户余额,type 0为减少，1为增加
                        var payResult = await shopApiService.SetBalanceAsync(buyerUid, 0, payAmount, sellEntity.CurrencyType);
                        if (!payResult)
                            return -6;//支付出错

                        //卖方增加账户余额
                        //修改商城账户余额,type 0为减少，1为增加
                        var getResult = await shopApiService.SetBalanceAsync(userSellEntity.ShopUID, 1, payAmount, sellEntity.CurrencyType);
                        if (!getResult)
                            return -7;//卖方账户增加出错

                        scope.Commit();
                        return order.Id;
                    }
                    catch (Exception ex)
                    {
                        scope.Rollback();
                        return -3;
                    }
                }
            }
        }
        //求购
        public async Task<long> AddAsync(long userId,long buyerUid, string orderCode, int number, decimal price, decimal amount, decimal charge, int balanceNum, int accountType)
        {
            decimal  payAmount = 0;
            payAmount = price * number;
            //获取商城账户余额
            var shopBalance = await shopApiService.GetBalanceAsync(buyerUid);
            if (shopBalance < payAmount)
                return -5;//账户余额不足

            using (MyDbContext dbc = new MyDbContext())
            {
                //修改商城账户余额,type 0为减少，1为增加
                var payResult = await shopApiService.SetBalanceAsync(buyerUid, 0, payAmount, accountType);
                if (!payResult)
                    return -6;//支付出错

                CashBuyEntity log = new CashBuyEntity();
                log.UserId = userId;
                log.OrderCode = orderCode;
                log.Number = number;
                log.Price = price;
                log.Amount = amount;
                log.Charge = charge;
                log.BalanceNum = balanceNum;
                log.StateType = 0;
                log.BuyType = 1;//BuyType 0:买入，1：求购
                log.CurrencyType = accountType;
                dbc.CashBuys.Add(log);
                await dbc.SaveChangesAsync();
                return log.Id;
            }
        }

        public async Task<CashBuySearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                CashBuySearchResult result = new CashBuySearchResult();
                var logs = dbc.GetAll<CashBuyEntity>().AsNoTracking();
                if (!string.IsNullOrEmpty(keyword))
                {
                    logs = logs.Where(a => a.OrderCode.Contains(keyword));
                }

                if (startTime != null)
                {
                    logs = logs.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    logs = logs.Where(a => SqlFunctions.DateDiff("day", endTime, a.CreateTime) <= 0);
                }
                result.PageCount = (int)Math.Ceiling((await logs.LongCountAsync()) * 1.0f / pageSize);
                var logsResult = await logs.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.log = logsResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }
        //求购订单
        public async Task<CashBuySearchResult> BuyListAsync(long? userId, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                CashBuySearchResult result = new CashBuySearchResult();
                var logs = dbc.GetAll<CashBuyEntity>().AsNoTracking();
                if (userId != null && userId > 0)
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
