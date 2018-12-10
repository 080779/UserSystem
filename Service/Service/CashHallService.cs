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
    public class CashHallService : ICashHallService
    {
        public IIdNameService idNameService { get; set; }
        public IJournalService journalService { get; set; }
        public IShopApiService shopApiService { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<CashHallSearchResult> GetListAsync(int accountType , int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                CashHallSearchResult result = new CashHallSearchResult();
                var buyEntity = dbc.GetAll<CashBuyEntity>().AsNoTracking()
                    .Where(w => w.StateType == 0 && w.BalanceNum > 0 && w.CurrencyType == accountType)
                    .Select(s => new CashHallDTO { TradeId = s.Id, TradeType = 1, TradeName = "买入积分", Num = s.BalanceNum, Amount = s.Amount, Price = s.Price, CreateTime = s.CreateTime,CurrencyType = s.CurrencyType });
              
                var sellEntity = dbc.GetAll<CashSellEntity>().AsNoTracking()
                    .Where(w => w.StateType == 0 && w.BalanceNum > 0 && w.CurrencyType == accountType)
                    .Select(s => new CashHallDTO { TradeId = s.Id, TradeType = 2, TradeName = "卖出积分", Num = s.BalanceNum, Amount = s.Amount, Price = s.Price, CreateTime = s.CreateTime, CurrencyType = s.CurrencyType });
                var list = buyEntity.Union(sellEntity);

                result.PageCount = (int)Math.Ceiling((await list.LongCountAsync()) * 1.0f / pageSize);
                var logsResult = await list.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.List = logsResult.Select(s => new CashHallDTO
                {
                    TradeId = s.TradeId,
                    TradeType = s.TradeType,
                    TradeName = s.TradeName,
                    Num = s.Num,
                    Amount = s.Amount,
                    Price = s.Price,
                    CreateTime = s.CreateTime,
                    CurrencyType = s.CurrencyType,
                    CurrencyName = s.CurrencyType.GetEnumName<CurrencyEnums>()
                }).ToArray();
                return result;
            }
        }
        public async Task<CashInfoDTO> GetInfoAsync()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                CashInfoDTO result = new CashInfoDTO();
                var buyCount = await dbc.GetAll<CashBuyEntity>().AsNoTracking()
                    .Where(w => w.StateType == 0 && w.BalanceNum > 0)
                    .CountAsync();

                var sellCount = await dbc.GetAll<CashSellEntity>().AsNoTracking()
                    .Where(w => w.StateType == 0 && w.BalanceNum > 0)
                    .CountAsync();

                DateTime startTime = DateTime.Parse(DateTime.Now.ToShortDateString());
                DateTime endTime = startTime.AddDays(1);
                var orderCount = await dbc.GetAll<CashOrderEntity>().AsNoTracking()
                    .Where(w => w.CreateTime >= startTime && SqlFunctions.DateDiff("day", endTime, w.CreateTime) <= 0
                    && w.StateType == (int)CashOrderStateEnums.已完成)
                    .CountAsync();

                result.WaitOrderQuantity = buyCount + sellCount;
                result.DayOrderQuantity = orderCount;
                return result;
            }
        }
        /// <summary>
        /// 获得A积分汇率
        /// </summary>
        /// <returns></returns>
        public async Task<decimal> GetExchangeAsync()
        {
            decimal price = 0;
            using (MyDbContext dbc = new MyDbContext())
            {
                //取积分价格记录的
                DateTime startTime = DateTime.Parse(DateTime.Now.ToShortDateString());
              
                var priceEntity = await dbc.GetAll<CashPriceEntity>().AsNoTracking()
                    .FirstOrDefaultAsync(w => SqlFunctions.DateDiff("day", startTime, w.CreateTime) == 0  );
                if (priceEntity == null)
                    price = 0;
                else
                    price = priceEntity.Price;

                if (price == 0)
                {
                    var settingEntity = await dbc.GetAll<SettingEntity>().AsNoTracking().FirstOrDefaultAsync(s=>s.Name== "默认A积分价格");
                    price = decimal.Parse(settingEntity.Param);
                }
                return price;
            }
        }

        public async Task<CashTradeRecordSearchResult> GetTradeRecordAsync(long userId, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                CashTradeRecordSearchResult result = new CashTradeRecordSearchResult();
                var buyEntity = dbc.GetAll<CashBuyEntity>().AsNoTracking()
                    .Where(w => w.UserId == userId)
                    .Select(s => new CashTradeRecordDTO { TradeId = s.Id, TradeType = 1, TradeName = "挂买",
                        Num = s.Number, Balance = s.BalanceNum, Price = s.Price,
                        CurrencyType = s.CurrencyType,
                        StateType = s.StateType, CreateTime = s.CreateTime });

                var sellEntity = dbc.GetAll<CashSellEntity>().AsNoTracking()
                    .Where(w =>  w.UserId == userId)
                    .Select(s => new CashTradeRecordDTO { TradeId = s.Id, TradeType = 2, TradeName = "挂卖",
                        Num = s.Number, Balance = s.BalanceNum, Price = s.Price,
                        CurrencyType = s.CurrencyType,
                        StateType = s.StateType,
                        CreateTime = s.CreateTime });
                var list = buyEntity.Union(sellEntity);

                result.PageCount = (int)Math.Ceiling((await list.LongCountAsync()) * 1.0f / pageSize);
                var logsResult = await list.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.List = logsResult.Select(a=>ToDTO(a)).ToArray();
                return result;
            }
        }
        private CashTradeRecordDTO ToDTO(CashTradeRecordDTO entity)
        {
            CashTradeRecordDTO dto = new CashTradeRecordDTO();
            dto.TradeId = entity.TradeId;
            dto.TradeType = entity.TradeType;
            dto.TradeName = entity.TradeName;
            dto.Num = entity.Num;
            dto.Balance = entity.Balance;
            dto.Price = entity.Price;
            dto.CurrencyType = entity.CurrencyType;
            dto.CurrencyName = entity.CurrencyType.GetEnumName<CurrencyEnums>();
            dto.StateType = entity.StateType;
            dto.StateName = entity.StateType.GetEnumName<CashSellStateEnums>();
            dto.CreateTime = entity.CreateTime;
            return dto;
        }
        /// <summary>
        /// 交易详情
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tradeId"></param>
        /// <param name="tradeType"></param>
        /// <returns></returns>
        public async Task<CashTradeDetailsDTO> GetTradeDetailsAsync(long userId, long tradeId, int tradeType)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                if (tradeType == 1)
                {
                    var buyEntity = await dbc.GetAll<CashBuyEntity>().AsNoTracking()
                        .Where(w => w.UserId == userId && w.Id == tradeId).ToListAsync();

                    var result =  buyEntity.Select(s => new CashTradeDetailsDTO
                    {
                        OrderCode = s.OrderCode,
                        TradeId = s.Id,
                        TradeType = 1,
                        TradeName = "挂买",
                        Num = s.Number,
                        Balance = s.BalanceNum,
                        Amount = s.Amount,
                        Price = s.Price,
                        CurrencyType = s.CurrencyType,
                        CurrencyName = s.CurrencyType.GetEnumName<CurrencyEnums>(),
                        StateType = s.StateType,
                        StateName = s.StateType.GetEnumName<CashBuyStateEnums>(),
                        CreateTime = s.CreateTime
                    }).FirstOrDefault();
                    return result;
                }
                else
                {
                    var sellEntity = await dbc.GetAll<CashSellEntity>().AsNoTracking()
                         .Where(w => w.UserId == userId && w.Id == tradeId).ToListAsync();

                    var result = sellEntity.Select(s => new CashTradeDetailsDTO
                    {
                        OrderCode = s.OrderCode,
                        TradeId = s.Id,
                        TradeType = 2,
                        TradeName = "挂卖",
                        Num = s.Number,
                        Balance = s.BalanceNum,
                        Amount = s.Amount,
                        Price = s.Price,
                        CurrencyType = s.CurrencyType,
                        CurrencyName = s.CurrencyType.GetEnumName<CurrencyEnums>(),
                        StateType = s.StateType,
                        StateName = s.StateType.GetEnumName<CashSellStateEnums>(),
                        CreateTime = s.CreateTime
                    }).FirstOrDefault();
                    return result;
                }
                
            }
        }

        /// <summary>
        /// 交易订单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tradeId"></param>
        /// <param name="tradeType"></param>
        /// <returns></returns>
        public async Task<CashTradeOrderSearchResult> GetTradeOrderAsync(long userId, long tradeId, int tradeType, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                CashTradeOrderSearchResult result = new CashTradeOrderSearchResult();
                if (tradeType == 1)
                {
                    var buyEntity = dbc.GetAll<CashOrderEntity>().AsNoTracking()
                        .Where(w => w.BuyUserId == userId && w.BuyId == tradeId);

                    var list = buyEntity.Select(s => new CashTradeOrderDTO
                    {
                        Num = s.Number,
                        Amount = s.Amount,
                        Trader = s.SellOrder.Seller.Mobile,
                        CreateTime = s.CreateTime
                    });
                    result.PageCount = (int)Math.Ceiling((await list.LongCountAsync()) * 1.0f / pageSize);
                    var logsResult = await list.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                    result.List = list.ToArray();

                    return result;
                }
                else
                {
                    var sellEntity = dbc.GetAll<CashOrderEntity>().AsNoTracking()
                         .Where(w => w.SellUserId == userId && w.SellId == tradeId);

                    var list = sellEntity.Select(s => new CashTradeOrderDTO
                    {
                        Num = s.Number,
                        Amount = s.Amount,
                        Trader = s.BuyOrder.Buyer.Mobile,
                        CreateTime = s.CreateTime
                    });
                    result.PageCount = (int)Math.Ceiling((await list.LongCountAsync()) * 1.0f / pageSize);
                    var logsResult = await list.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                    result.List = list.ToArray();

                    return result;
                }

            }
        }

        /// <summary>
        /// 订单记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tradeId"></param>
        /// <param name="tradeType"></param>
        /// <returns></returns>
        public async Task<CashOrderRecordSearchResult> GetOrderRecordAsync(long userId, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                CashOrderRecordSearchResult result = new CashOrderRecordSearchResult();

                var buyEntity = dbc.GetAll<CashOrderEntity>().AsNoTracking()
                    .Where(w => w.BuyUserId == userId || w.SellUserId == userId);

                var list = buyEntity.Select(s => new CashOrderRecordDTO
                {
                    OrderType = s.BuyUserId == userId ? "购买" : "出售",
                    Num = s.Number,
                    Price = s.Price,
                    Amount = s.Amount,
                    Trader = s.BuyUserId == userId ? s.SellOrder.Seller.Mobile : s.BuyOrder.Buyer.Mobile,
                    CreateTime = s.CreateTime
                });
                result.PageCount = (int)Math.Ceiling((await list.LongCountAsync()) * 1.0f / pageSize);
                var logsResult = await list.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.List = list.ToArray();

                return result;

            }
        }
        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<int> GetTradeCloseAsync(long userId, long tradeId, int tradeType)
        {
            decimal balanceAmount = 0;

            using (MyDbContext dbc = new MyDbContext())
            {
                if (tradeType == 1)
                {
                    var buyEntity = await dbc.GetAll<CashBuyEntity>()
                        .SingleOrDefaultAsync(w => w.UserId == userId && w.Id == tradeId);

                    buyEntity.StateType = (int)CashBuyStateEnums.已关闭;

                    if(buyEntity.BalanceNum > 0)
                    {
                        //关闭订单 退回账户余额
                        //修改商城账户余额,type 0为减少，1为增加
                        var payResult = await shopApiService.SetBalanceAsync(buyEntity.Buyer.ShopUID, 1, buyEntity.BalanceNum * buyEntity.Price, buyEntity.CurrencyType);
                        if (!payResult)
                            return -1;//关闭订单出错
                    }
                }
                else if (tradeType == 2)
                {
                    var sellEntity = await dbc.GetAll<CashSellEntity>()
                        .SingleOrDefaultAsync(w => w.UserId == userId && w.Id == tradeId);

                    sellEntity.StateType = (int)CashSellStateEnums.已关闭;

                    if (sellEntity.BalanceNum > 0)
                    {
                        var userSellEntity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(w => w.Id == userId);
                        if (sellEntity.CurrencyType == 1)
                        {
                            userSellEntity.BonusAmount += sellEntity.BalanceNum;
                            balanceAmount = userSellEntity.BonusAmount;
                        }
                        else
                        {
                            userSellEntity.Amount += sellEntity.BalanceNum;
                            balanceAmount = userSellEntity.Amount;
                        }

                        int JournalTypeId = (int)JournalTypeEnum.关闭卖出订单;
                        await journalService.AddAsync(userSellEntity.Id, sellEntity.BalanceNum, JournalTypeId, sellEntity.CurrencyType, balanceAmount, "关闭卖出订单（"+ sellEntity.OrderCode + "）,退回积分", 1);
                    }

                }

                return await dbc.SaveChangesAsync();
            }
        }
    }
}
