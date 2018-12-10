using IMS.Common;
using IMS.Common.Enums;
using IMS.DTO;
using IMS.IService;
using IMS.Service.Entity;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Service
{
    public class BonusService : IBonusService
    {
        private static ILog log = LogManager.GetLogger(typeof(OrderService));
        public ICashHallService cashHallService { get; set; }
        public BonusDTO ToDTO(BonusEntity entity)
        {
            BonusDTO dto = new BonusDTO();
            dto.Source = entity.Source;
            dto.TypeID = entity.TypeID;
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.Amount = entity.Amount;
            dto.Revenue = entity.Revenue;
            dto.sf = entity.sf;
            dto.UserId = entity.UserId;
            dto.IsSettled = entity.IsSettled;
            dto.SttleTime = entity.SttleTime;
            dto.FromUserID = entity.FromUserID;
            dto.UserMobile = entity.User.Mobile;
            return dto;
        }
        public async Task<long> AddAsync(long userId, decimal amount, decimal revenue, decimal sf, string source, long fromUserID, int isSettled)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                BonusEntity entity = new BonusEntity();
                entity.UserId = userId;
                entity.Amount = amount;
                entity.Revenue = revenue;
                entity.sf = sf;
                entity.Source = source;
                entity.FromUserID = fromUserID;
                entity.IsSettled = isSettled;
                dbc.Bonus.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                BonusEntity entity = await dbc.GetAll<BonusEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<BonusDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<BonusEntity>().AsNoTracking().SingleOrDefaultAsync(a => a.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<BonusDTO> GetDefaultModelAsync(long userId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<BonusEntity>().AsNoTracking().SingleOrDefaultAsync(a => a.UserId == userId);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<BonusSearchResult> GetModelListAsync(long? userId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                BonusSearchResult result = new BonusSearchResult();
                var entities = dbc.GetAll<BonusEntity>().AsNoTracking();
                if (userId != null)
                {
                    entities = entities.Where(a => a.UserId == userId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Source.Contains(keyword));
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
                var addressResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.List = addressResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }

        }

        /// <summary>
        /// 计算直推奖奖金
        /// </summary>
        /// <param name="dbc"></param>
        /// <param name="orderId"></param>
        /// <param name="orderCode"></param>
        /// <param name="amount">支付积分部分</param>
        /// <param name="user"></param>
        public async Task<bool> CalcAwardRecommend(long orderId, string orderCode, decimal amount, long buyerId)
        {
            decimal bonusRate = 0;
            string bonusName = string.Empty;
            if (amount <= 0)
                return true;

           // decimal aPrice = await cashHallService.GetExchangeAsync();
           // amount = amount / aPrice;//转换为 A积分

            using (MyDbContext dbc = new MyDbContext())
            {
                int journalTypeId = 1;

                var buyerEntity = await dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == buyerId);
                long iRecId = buyerEntity.RecommendId;// 
                long iRecIdTemp = 0;
                int genera = 1;

                decimal bonus = 0;

                while (genera < 3 && iRecId > 0)
                {
                    var recEntity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == iRecId);
                    iRecIdTemp = recEntity.RecommendId;

                    if (genera == 1)
                        bonusName = "一级直推奖";
                    else if (genera == 2)
                        bonusName = "二级直推奖";

                    SettingEntity bonusRatio = await dbc.GetAll<SettingEntity>().AsNoTracking().SingleOrDefaultAsync(b => b.Name == bonusName);
                    decimal.TryParse(bonusRatio.Param, out bonusRate);
                    bonus = amount * bonusRate / 100;

                    recEntity.BonusAmount += bonus; //A积分

                    BonusEntity entity = new BonusEntity();
                    entity.UserId = recEntity.Id;
                    entity.Amount = bonus;
                    entity.Revenue = 0;
                    entity.sf = bonus;
                    entity.TypeID = 1; //直推奖
                    entity.Source = "用户(" + buyerEntity.Mobile + ")购买商品,获得直推奖";
                    entity.FromUserID = buyerEntity.Id;
                    entity.IsSettled = 1;
                    dbc.Bonus.Add(entity);

                    JournalEntity journal1 = new JournalEntity();
                    journal1.UserId = recEntity.Id;
                    journal1.BalanceAmount = recEntity.BonusAmount;
                    journal1.InAmount = bonus;
                    journal1.Remark = "用户(" + buyerEntity.Mobile + ")购买商品,获得直推奖";
                    journal1.JournalTypeId = journalTypeId;
                    journal1.OrderCode = orderCode;
                    journal1.GoodsId = orderId;//来至订单ID
                    journal1.CurrencyType = (int)CurrencyEnums.碳积分;
                    dbc.Journals.Add(journal1);

                    iRecId = iRecIdTemp;
                    genera++;

                    //累计业绩
                    int result = await dbc.Database.ExecuteSqlCommandAsync("exec proc_Sroce @UserId = " + recEntity.Id + ",@sroce = " + bonus);

                }
                await dbc.SaveChangesAsync();
            }
            return true;
        }
        //极差奖
        public async Task<bool> CalcAwardLevelDiff(long orderId, string orderCode, decimal amount, long buyerId)
        {
            decimal lastRate = 0, currRate = 0, bonusRate = 0;
            decimal bonus = 0;
            string bonusName = string.Empty,flagName ;
            long iRecIdTemp = 0;
           
            long _level = 0,_mLevel = 0 ;

            using (MyDbContext dbc = new MyDbContext())
            {
                try
                {
                    int journalTypeId = 1;

                    var buyerEntity = await dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == buyerId);
                    long iRecId = buyerEntity.RecommendId;// 

                    while (iRecId > 0)
                    {
                        var recEntity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == iRecId);
                        iRecIdTemp = recEntity.RecommendId;
                        _mLevel = recEntity.MLevelId; //管理级别
                        _level = recEntity.LevelId;  //会员级别
                        if (_mLevel == 0)
                        {
                            iRecId = iRecIdTemp;
                            continue;
                        }

                        decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.ParamName == "极差奖金比例" && s.LevelId == _mLevel, s => s.Param), out currRate);
                        if (currRate > lastRate)
                        {
                            bonusRate = currRate - lastRate;
                            lastRate = currRate;
                        }
                        else bonusRate = 0;

                        if (bonusRate > 0)
                        {
                            decimal bonusTopRate = 0, bonusTop = 0;
                            decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.ParamName == "极差奖额度" && s.LevelId == _level, s => s.Param), out bonusTopRate);
                            decimal bonusAll = await dbc.GetAll<BonusEntity>().Where(b => b.UserId == recEntity.Id && b.TypeID == 4).SumAsync(b => (decimal?)b.sf) ?? 0;

                            bonusTop = recEntity.PersonalScore * bonusTopRate / 100;

                            bonus = amount * bonusRate / 100;
                            flagName = string.Empty;

                            if (bonusAll + bonus > bonusTop) //封顶
                            {
                                flagName = "(封顶)";
                                bonus = bonusTop - bonusAll;
                            }

                            if (bonus > 0)
                            {
                                recEntity.BonusAmount += bonus; //A积分
                                recEntity.BonusDiffTotal += bonus;//累计

                                BonusEntity entity = new BonusEntity();
                                entity.UserId = recEntity.Id;
                                entity.Amount = bonus;
                                entity.Revenue = 0;
                                entity.sf = bonus;
                                entity.TypeID = 4; //极差奖
                                entity.Source = "用户(" + buyerEntity.Mobile + ")购买商品,获得极差奖" + flagName;
                                entity.FromUserID = buyerEntity.Id;
                                entity.IsSettled = 1;
                                dbc.Bonus.Add(entity);

                                JournalEntity journal1 = new JournalEntity();
                                journal1.UserId = recEntity.Id;
                                journal1.BalanceAmount = recEntity.BonusAmount;
                                journal1.InAmount = bonus;
                                journal1.Remark = "用户(" + buyerEntity.Mobile + ")购买商品,获得极差奖" + flagName;
                                journal1.JournalTypeId = journalTypeId;
                                journal1.OrderCode = orderCode;
                                journal1.GoodsId = orderId;//来至订单ID
                                journal1.CurrencyType = (int)CurrencyEnums.碳积分;
                                dbc.Journals.Add(journal1);

                                //累计业绩
                                int result = await dbc.Database.ExecuteSqlCommandAsync("exec proc_Sroce @UserId = " + recEntity.Id + ",@sroce = " + bonus);

                            }
                            if (_mLevel == 6)//最高级别
                                break;
                        }
                        iRecId = iRecIdTemp;
                    }
                    await dbc.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    log.ErrorFormat($"极差奖 CalcAwardLevelDiff Exception:{ex.ToString()},orderCode:{orderCode}，buyerId：{buyerId}");
                    return false;
                }
            }
            return true;
        }
        public class SettingModel
        {
            public long? LevelId { get; set; }
            public string Parm { get; set; }
        }
        public class UserModel
        {
            public long Id { get; set; }
            public long LevelId { get; set; }
            public decimal BonusDiffTotal { get; set; }
            public decimal PersonalScore { get; set; }
        }
        //极差级别达到最高级别800000时，额外赠送平台新增积分的10%，所有有资格领取的用户平分
        public async Task<bool> CalcAwardLevelDiffMaxAvg(long orderId, string orderCode, decimal amount, long buyerId,string buyMobile)
        {
            decimal bonus = 0;
            string flagName = string.Empty;

            using (MyDbContext dbc = new MyDbContext())
            {
                decimal bonusRate = 0, bonusAvg = 0, bonusSf = 0;
                decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.ParamName == "最高极差奖级别平分", s => s.Param), out bonusRate);
                int journalTypeId = 1;
                bonus = amount * bonusRate / 100;
                //获奖用户
                var settingEntity = dbc.GetAll<SettingEntity>().AsNoTracking().Where(w => w.ParamName == "极差奖额度")
                    .Select(s => new SettingModel { LevelId = s.LevelId, Parm = s.Param }).ToList();
                var userEntity =  dbc.GetAll<UserEntity>().AsNoTracking()
                    .Where(u => u.IsNull == false && u.MLevelId == 6 && u.LevelId < 10)
                    .Select(s=>new UserModel { Id = s.Id, LevelId = s.LevelId }).ToList();

                var yy = userEntity.Join(settingEntity, u => u.LevelId, x => x.LevelId, (u, x) => new {u,x})
                    .Where(y=>y.u.BonusDiffTotal < y.u.PersonalScore * Convert.ToDecimal(y.x.Parm) / 100  )
                    .Select(s=> new { s.u.Id,balance = s.u.PersonalScore * Convert.ToDecimal(s.x.Parm) / 100  - s.u.BonusDiffTotal }).ToList();
                if (yy.Count == 0) return false;

                bonusAvg = bonus / yy.Count; //平分
                foreach (var item in yy)
                {
                    if (item.balance > bonusAvg)
                    {
                        bonusSf = bonusAvg;
                    }
                    else
                    {
                        bonusSf = item.balance;
                        flagName = "(封顶)";
                    }

                    var userBonusEntity = await dbc.GetAll<UserEntity>().SingleAsync(s => s.Id == item.Id);
                    userBonusEntity.BonusAmount += bonus;
                    userBonusEntity.BonusDiffTotal += bonus;//累计

                    BonusEntity entity = new BonusEntity();
                    entity.UserId = item.Id;
                    entity.Amount = bonus;
                    entity.Revenue = 0;
                    entity.sf = bonus;
                    entity.TypeID = 4; //极差奖
                    entity.Source = "用户(" + buyMobile + ")购买商品,最高极差奖级别获得极差奖" + flagName;
                    entity.FromUserID = buyerId;
                    entity.IsSettled = 1;
                    dbc.Bonus.Add(entity);


                    JournalEntity journal1 = new JournalEntity();
                    journal1.UserId = item.Id;
                    journal1.BalanceAmount = userBonusEntity.BonusAmount;
                    journal1.InAmount = bonus;
                    journal1.Remark = "用户(" + buyMobile + ")购买商品,最高极差奖级别获得极差奖" + flagName;
                    journal1.JournalTypeId = journalTypeId;
                    journal1.OrderCode = orderCode;
                    journal1.GoodsId = orderId;//来至订单ID
                    journal1.CurrencyType = (int)CurrencyEnums.碳积分;
                    dbc.Journals.Add(journal1);

                    //累计业绩
                    int result = await dbc.Database.ExecuteSqlCommandAsync("exec proc_Sroce @UserId = " + item.Id + ",@sroce = " + bonus);

                }
                if (yy.Count > 0)
                    await dbc.SaveChangesAsync();

                return true;
            }
        }
        /// <summary>
        /// 计算购物奖
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderCode"></param>
        /// <param name="profit">支付积分部分</param>
        /// <param name="buyerId"></param>
        /// <returns></returns>
        public async Task<bool> CalcAwardBuy(long orderId, string orderCode, decimal profit, long buyerId)
        {
            if (profit <= 0)
            {
                return true;
            }
            using (MyDbContext dbc = new MyDbContext())
            {
                int journalTypeId = 1;
                decimal parm;
                decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.Name == "购物奖励", s => s.Param), out parm);
                var user = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == buyerId);
                decimal bonus = profit * (parm / 100);

                user.FrozenAmount = user.FrozenAmount + bonus;

                BonusEntity entity = new BonusEntity();
                entity.UserId = user.Id;
                entity.Amount = bonus;
                entity.Revenue = 0;
                entity.sf = bonus;
                entity.TypeID = 3; //购物奖励
                entity.Source = "用户(" + user.Mobile + ")购买商品,获得购物奖励";
                entity.FromUserID = user.Id;
                entity.IsSettled = 1;
                dbc.Bonus.Add(entity);

                JournalEntity journal1 = new JournalEntity();
                journal1.UserId = user.Id;
                journal1.BalanceAmount = user.FrozenAmount;
                journal1.InAmount = bonus;
                journal1.Remark = "用户(" + user.Mobile + ")购买商品,获得购物奖励";
                journal1.JournalTypeId = journalTypeId;
                journal1.OrderCode = orderCode;
                journal1.GoodsId = orderId;//来至订单ID
                journal1.CurrencyType = (int)CurrencyEnums.碳积分;
                dbc.Journals.Add(journal1);
                await dbc.SaveChangesAsync();

                //累计业绩
                int flag = await dbc.Database.ExecuteSqlCommandAsync("exec proc_Sroce @UserId = " + user.Id + ",@sroce = " + bonus);
                log.DebugFormat($"购物奖励 累计业绩 CalcAwardBuy:{flag},orderCode:{orderCode}，buyerId：{buyerId}");
                //极差奖
                bool result = await CalcAwardLevelDiff(orderId, orderCode, bonus, buyerId);
                log.DebugFormat($"购物奖励 极差奖 CalcAwardLevelDiff:{result},orderCode:{orderCode}，buyerId：{buyerId}");
                //极差奖 平分
                result = await CalcAwardLevelDiffMaxAvg(orderId, orderCode, bonus, buyerId, user.Mobile);
                log.DebugFormat($"购物奖励 极差奖 平分 CalcAwardLevelDiffMaxAvg:{result},orderCode:{orderCode}，buyerId：{buyerId}");

                return true;
            }
        }

        /// <summary>
        /// 计算静态奖
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderCode"></param>
        /// <param name="buyerId"></param>
        /// <returns></returns>
        public async Task<bool> CalcStaticAward(long orderId, string orderCode, long buyerId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                decimal aPrice = await cashHallService.GetExchangeAsync();
                decimal bonus = 0;
                decimal parm;
                var user = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == buyerId);
                int? prizeLevel = await dbc.GetAll<BonusEntity>().Where(b => b.UserId == buyerId && b.TypeID == 2).MaxAsync(b => b.Bonus001);
                decimal amount = user.PersonalScore;
                decimal totalAmount1, totalAmount2, totalAmount3, totalAmount4;
                decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.Name == "订单金额累计等级一额度", s => s.Param), out totalAmount1);
                decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.Name == "订单金额累计等级二额度", s => s.Param), out totalAmount2);
                decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.Name == "订单金额累计等级三额度", s => s.Param), out totalAmount3);
                decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.Name == "订单金额累计等级四额度", s => s.Param), out totalAmount4);

                if (amount < totalAmount1)
                {
                    return true;
                }
                else if (prizeLevel == 4)
                {
                    return true;
                }
                else if(prizeLevel == 0 || prizeLevel == null)
                {
                    if(amount >= totalAmount4)
                    {
                        decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.Name == "订单金额累计等级四比例", s => s.Param), out parm);
                        bonus = parm * totalAmount4 / 100;
                        prizeLevel = 4;
                    }
                    else if(amount >= totalAmount3 && amount < totalAmount4)
                    {
                        decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.Name == "订单金额累计等级三比例", s => s.Param), out parm);
                        bonus = parm * totalAmount3 / 100;
                        prizeLevel = 3;
                    }
                    else if(amount >= totalAmount2 && amount < totalAmount3)
                    {
                        decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.Name == "订单金额累计等级二比例", s => s.Param), out parm);
                        bonus = parm * totalAmount2 / 100;
                        prizeLevel = 2;
                    }
                    else if (amount >= totalAmount1 && amount < totalAmount2)
                    {
                        decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.Name == "订单金额累计等级一比例", s => s.Param), out parm);
                        bonus = parm * totalAmount1 / 100;
                        prizeLevel = 1;
                    }
                }
                else if(prizeLevel == 1)
                {
                    if (amount >= totalAmount4)
                    {
                        decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.Name == "订单金额累计等级四比例", s => s.Param), out parm);
                        bonus = parm * (totalAmount4 - totalAmount1) / 100;
                        prizeLevel = 4;
                    }
                    else if (amount >= totalAmount3 && amount < totalAmount4)
                    {
                        decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.Name == "订单金额累计等级三比例", s => s.Param), out parm);
                        bonus = parm * (totalAmount3 - totalAmount1) / 100;
                        prizeLevel = 3;
                    }
                    else if (amount >= totalAmount2 && amount < totalAmount3)
                    {
                        decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.Name == "订单金额累计等级二比例", s => s.Param), out parm);
                        bonus = parm * (totalAmount2 - totalAmount1) / 100;
                        prizeLevel = 2;
                    }
                }
                else if (prizeLevel == 2)
                {
                    if (amount >= totalAmount4)
                    {
                        decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.Name == "订单金额累计等级四比例", s => s.Param), out parm);
                        bonus = parm * (totalAmount4 - totalAmount2) / 100;
                        prizeLevel = 4;
                    }
                    else if (amount >= totalAmount3 && amount < totalAmount4)
                    {
                        decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.Name == "订单金额累计等级三比例", s => s.Param), out parm);
                        bonus = parm * (totalAmount3 - totalAmount2) / 100;
                        prizeLevel = 3;
                    }
                }
                else if (prizeLevel == 3)
                {
                    if (amount >= totalAmount4)
                    {
                        decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.Name == "订单金额累计等级四比例", s => s.Param), out parm);
                        bonus = parm * (totalAmount4 - totalAmount3) / 100;
                        prizeLevel = 4;
                    }                    
                }
                //else if (amount >= totalAmount1 && amount < totalAmount2)
                //{
                //    decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.Name == "订单金额累计等级一比例", s => s.Param), out parm);
                //    bonus = parm * totalAmount1 / 100;
                //    prizeLevel = 1;
                //}
                //else if (amount >= totalAmount2 && amount < totalAmount3 && prizeLevel == 1)
                //{
                //    decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.Name == "订单金额累计等级二比例", s => s.Param), out parm);
                //    bonus = parm * (totalAmount2 - totalAmount1) / 100;
                //    prizeLevel++;
                //}
                //else if (amount >= totalAmount3 && amount < totalAmount4 && prizeLevel == 2)
                //{
                //    decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.Name == "订单金额累计等级三比例", s => s.Param), out parm);
                //    bonus = parm * (totalAmount3 - totalAmount2) / 100;
                //    prizeLevel++;
                //}
                //else if (amount >= totalAmount4  && prizeLevel == 3)
                //{
                //    decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.Name == "订单金额累计等级四比例", s => s.Param), out parm);
                //    bonus = parm * (totalAmount4 - totalAmount3) / 100;
                //    prizeLevel++;
                //}
                int _level = 1;
                bonus = bonus / aPrice; //换算价值

                int journalTypeId = 1;
                user.FrozenAmount = user.FrozenAmount + bonus;
                user.LevelId = _level;

                BonusEntity entity = new BonusEntity();
                entity.UserId = user.Id;
                entity.Amount = bonus;
                entity.Revenue = 0;
                entity.sf = bonus;
                entity.TypeID = 2; //静态奖
                entity.Source = "用户(" + user.Mobile + ")购买商品,获得静态奖";
                entity.FromUserID = user.Id;
                entity.IsSettled = 1;
                entity.Bonus001 = prizeLevel; //1、发了第一个差额的奖 2、发了第二个差额的奖 3、发了第三个差额的奖 4、发了第四个差额的奖
                dbc.Bonus.Add(entity);

                JournalEntity journal1 = new JournalEntity();
                journal1.UserId = user.Id;
                journal1.BalanceAmount = user.FrozenAmount;
                journal1.InAmount = bonus;
                journal1.Remark = "用户(" + user.Mobile + ")购买商品,获得静态奖";
                journal1.JournalTypeId = journalTypeId;
                journal1.OrderCode = orderCode;
                journal1.GoodsId = orderId;//来至订单ID
                journal1.CurrencyType = (int)CurrencyEnums.碳积分;
                dbc.Journals.Add(journal1);
                await dbc.SaveChangesAsync();
                
                //累计业绩
                int flag = await dbc.Database.ExecuteSqlCommandAsync("exec proc_Sroce @UserId = " + user.Id + ",@sroce = " + bonus);
                log.DebugFormat($"静态奖 累计业绩 proc_Sroce:{flag},orderCode:{orderCode}，buyerId：{buyerId}");
                //推荐奖
                bool result = await CalcAwardRecommend(orderId, orderCode, bonus, buyerId);
                log.DebugFormat($"静态奖 推荐奖 CalcAwardRecommend:{result},orderCode:{orderCode}，buyerId：{buyerId}");
                //极差奖
                result = await CalcAwardLevelDiff(orderId, orderCode, bonus, buyerId);
                log.DebugFormat($"静态奖 极差奖 CalcAwardLevelDiff:{result},orderCode:{orderCode}，buyerId：{buyerId}");
                //极差奖 平分
                result = await CalcAwardLevelDiffMaxAvg(orderId, orderCode, bonus, buyerId, user.Mobile);
                log.DebugFormat($"静态奖 极差奖 平分 CalcAwardLevelDiffMaxAvg:{result},orderCode:{orderCode}，buyerId：{buyerId}");

                return true;
            }
        }

        //奖金发放记录
        public async Task<BonusSearchResult> BoussListAsync(long? userId, long? typeId, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                BonusSearchResult result = new BonusSearchResult();
                var logs = dbc.GetAll<BonusEntity>().AsNoTracking();
                if (userId != null && userId > 0)
                {
                    logs = logs.Where(a => a.UserId == userId);
                }
                if (typeId != null && typeId > 0)
                {
                    logs = logs.Where(a => a.TypeID == typeId);
                }
                result.PageCount = (int)Math.Ceiling((await logs.LongCountAsync()) * 1.0f / pageSize);
                var logsResult = await logs.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.List = logsResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

    }
}
