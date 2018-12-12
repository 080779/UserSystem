using IMS.Common;
using IMS.Common.Enums;
using IMS.DTO;
using IMS.IService;
using IMS.Service.Entity;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Service
{
    public class UserService : IUserService
    {
        private static ILog log = LogManager.GetLogger(typeof(UserService));
        public IIdNameService idNameService { get; set; }
        public IJournalService journalService { get; set; }
        public ICashHallService cashHallService { get; set; }
        public UserDTO ToDTO(UserEntity entity)
        {
            UserDTO dto = new UserDTO();
            dto.ShopUID = entity.ShopUID;
            dto.UserCode = entity.UserCode;
            dto.Amount = entity.Amount;
            dto.Code = entity.Code;
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.ErrorCount = entity.ErrorCount;
            dto.ErrorTime = entity.ErrorTime;
            dto.Id = entity.Id;
            dto.IsEnabled = entity.IsEnabled;
            dto.LevelId = entity.LevelId;
            dto.LevelName = entity.LevelId.GetEnumName<LevelEnum>();
            dto.Mobile = entity.Mobile;
            dto.NickName = entity.NickName;
            dto.BuyAmount = entity.BuyAmount;
            dto.IsReturned = entity.IsReturned;
            dto.IsUpgraded = entity.IsUpgraded;
            dto.BonusAmount = entity.BonusAmount;
            dto.HeadPic = entity.HeadPic;
            dto.ShareCode = entity.ShareCode;
            dto.FrozenAmount = entity.FrozenAmount;
            dto.RecommendId = entity.RecommendId;
            dto.RecommendCode = entity.RecommendCode;
            dto.RecommendPath = entity.RecommendPath;
            dto.RecommendGenera = entity.RecommendGenera;
            dto.RegMoney = entity.RegMoney;
            dto.TeamScore = entity.TeamScore;
            dto.PersonalScore = entity.PersonalScore;
            dto.MLevelId = entity.MLevelId;
            dto.BonusDiffTotal = entity.BonusDiffTotal;
            dto.TrueName = entity.TrueName;
            return dto;
        }

        public UserRecommendTreeDTO ToDTO(UserEntity entity,int mLevelId)
        {            
            UserRecommendTreeDTO dto = new UserRecommendTreeDTO();
            dto.Id = entity.Id;
            dto.Mobile = entity.Mobile;
            dto.Amount = entity.Amount;
            dto.ShopUID = entity.ShopUID;
            dto.LevelName = entity.LevelId.GetEnumName<LevelEnum>();
            return dto;
        }

        public async Task<long> AddAsync(string mobile, int levelTypeId, string password, string tradePassword, string recommend, string nickName,string avatarUrl)
        {
            string userCode = string.Empty;

            using (MyDbContext dbc = new MyDbContext())
            {
                long userId = 0;
                do
                {
                    userCode = CommonHelper.GetNumberCaptcha(6);
                    userId = await dbc.GetIdAsync<UserEntity>(u=>u.Code==userCode);
                } while (userId != 0);

                UserEntity recUser;
                if (string.IsNullOrWhiteSpace(recommend))
                {
                    recUser = await dbc.GetAll<UserEntity>().AsNoTracking().SingleOrDefaultAsync(u => u.Id == 1);
                }
                else
                {
                    recUser = await dbc.GetAll<UserEntity>().AsNoTracking().SingleOrDefaultAsync(u => u.UserCode == recommend);
                }   
                
                if (recUser == null)
                {
                    return -1;
                }

                if(!recUser.IsUpgraded)
                {
                    return -4;
                }

                if ((await dbc.GetIdAsync<UserEntity>(u => u.Mobile == mobile)) > 0)
                {
                    return -2;
                }

                try
                {
                    UserEntity user = new UserEntity();
                    user.UserCode = userCode;
                    user.LevelId = levelTypeId;
                    user.Mobile = mobile;
                    user.Salt = CommonHelper.GetCaptcha(4);
                    user.Password = CommonHelper.GetMD5(password + user.Salt);
                    //user.TradePassword = "";// tradePassword;// CommonHelper.GetMD5(tradePassword + user.Salt);
                    user.NickName = string.IsNullOrEmpty(nickName) ? "无昵称" : nickName;
                    user.HeadPic =  string.IsNullOrEmpty(avatarUrl)? "/images/headpic.png" : avatarUrl;

                    user.RecommendId = recUser.Id;
                    user.RecommendGenera = recUser.RecommendGenera + 1;
                    user.RecommendPath = recUser.RecommendPath;
                    user.RecommendCode = recUser.Mobile;

                    dbc.Users.Add(user);
                    await dbc.SaveChangesAsync();

                    var userModel = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(s => s.Id == user.Id);
                    user.RecommendPath = user.RecommendPath + "-" + user.Id;
                    await dbc.SaveChangesAsync();
                    return user.Id;
                }
                catch (Exception ex)
                {
                    //  scope.Rollback();
                    log.ErrorFormat("AddAsync:{0}", ex.ToString());
                    return -3;
                }
            }
        }

        public async Task<long> AddByExcelAsync(string mobile,string trueName, int levelTypeId, string password, string tradePassword, string recommend, string nickName, string avatarUrl)
        {
            string userCode = string.Empty;

            using (MyDbContext dbc = new MyDbContext())
            {
                long userId = 0;
                do
                {
                    userCode = CommonHelper.GetNumberCaptcha(6);
                    userId = await dbc.GetIdAsync<UserEntity>(u => u.Code == userCode);
                } while (userId != 0);

                UserEntity recUser;
                if (string.IsNullOrWhiteSpace(recommend))
                {
                    recUser = await dbc.GetAll<UserEntity>().AsNoTracking().SingleOrDefaultAsync(u => u.Id == 1);
                }
                else
                {
                    recUser = await dbc.GetAll<UserEntity>().AsNoTracking().SingleOrDefaultAsync(u => u.TrueName == recommend);
                }

                if (recUser == null)
                {
                    return -1;
                }

                //if (!recUser.IsUpgraded)
                //{
                //    return -4;
                //}

                if ((await dbc.GetIdAsync<UserEntity>(u => u.Mobile == mobile)) > 0)
                {
                    return -2;
                }

                try
                {
                    UserEntity user = new UserEntity();
                    user.UserCode = userCode;
                    user.LevelId = levelTypeId;
                    user.Mobile = mobile;
                    user.Salt = CommonHelper.GetCaptcha(4);
                    user.Password = CommonHelper.GetMD5(password + user.Salt);
                    //user.TradePassword = "";// tradePassword;// CommonHelper.GetMD5(tradePassword + user.Salt);
                    user.NickName = string.IsNullOrEmpty(nickName) ? "无昵称" : nickName;
                    user.HeadPic = string.IsNullOrEmpty(avatarUrl) ? "/images/headpic.png" : avatarUrl;
                    user.TrueName = trueName;

                    user.RecommendId = recUser.Id;
                    user.RecommendGenera = recUser.RecommendGenera + 1;
                    user.RecommendPath = recUser.RecommendPath;
                    user.RecommendCode = recUser.Mobile;

                    dbc.Users.Add(user);
                    await dbc.SaveChangesAsync();

                    var userModel = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(s => s.Id == user.Id);
                    user.RecommendPath = user.RecommendPath + "-" + user.Id;
                    await dbc.SaveChangesAsync();
                    return user.Id;
                }
                catch (Exception ex)
                {
                    //  scope.Rollback();
                    log.ErrorFormat("AddAsync:{0}", ex.ToString());
                    return -3;
                }
            }
        }

        public async Task<bool> AddAmountAsync(string mobile, decimal amount)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Mobile == mobile);
                if (user == null)
                {
                    return false;
                }
                user.Amount = user.Amount + amount;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> AddAntegralAsync(long id, int integral)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return false;
                }
                user.Amount = user.Amount + integral;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateInfoAsync(long id, string nickName, string headpic,string trueName)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return false;
                }
                if (!string.IsNullOrEmpty(nickName))
                {
                    entity.NickName = nickName;
                }
                if (!string.IsNullOrEmpty(headpic))
                {
                    entity.HeadPic = headpic;
                }
                if(!string.IsNullOrEmpty(trueName))
                {
                    entity.TrueName = trueName;
                }
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateShareCodeAsync(long id, string codeUrl)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.ShareCode = codeUrl;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateTeamShareCodeAsync(long id, string codeUrl)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                RegisterEntity entity = await dbc.GetAll<RegisterEntity>().SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.TeamShareCode = codeUrl;
                await dbc.SaveChangesAsync();
                return true;
            }
        }
        public async Task<bool> UpdatePhone(long id, string phone)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == id);
                MemberEntity memberEntity = await dbc.GetAll<MemberEntity>().FirstOrDefaultAsync(u => u.Mobile == phone);
                if (entity == null)
                {
                    return false;
                }
                entity.Mobile = phone;


                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> ActivateAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return false;
                }
                //激活会员
                entity.IsUpgraded = true;
                entity.Amount = entity.Amount + 100;

                BonusEntity bonus = new BonusEntity();
                bonus.UserId = entity.Id;
                bonus.Amount = 100;
                bonus.Revenue = 0;
                bonus.sf = 100;
                bonus.TypeID = 1; //购买课程奖励
                bonus.Source = "用户(" + entity.Mobile + ")后台激活成功，获赠碳积分";
                bonus.FromUserID = entity.Id;
                bonus.IsSettled = 1;
                dbc.Bonus.Add(bonus);

                JournalEntity journal = new JournalEntity();
                journal.UserId = entity.Id;
                journal.BalanceAmount = entity.Amount;
                journal.InAmount = 100;
                journal.Remark = "用户(" + entity.Mobile + ")后台激活成功，获赠碳积分";
                journal.JournalTypeId = (int)JournalTypeEnum.会员激活;
                journal.OrderCode = "";
                journal.GoodsId = 0;//来至订单ID
                journal.CurrencyType = (int)CurrencyEnums.碳积分;
                dbc.Journals.Add(journal);
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> ActivateAllAsync(long[] ids)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                //批量激活会员
                foreach (var id in ids)
                {
                    UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == id);
                    if (entity == null)
                    {
                        return false;
                    }
                    entity.IsUpgraded = true;
                    entity.Amount = entity.Amount + 100;

                    BonusEntity bonus = new BonusEntity();
                    bonus.UserId = entity.Id;
                    bonus.Amount = 100;
                    bonus.Revenue = 0;
                    bonus.sf = 100;
                    bonus.TypeID = 1; //购买课程奖励
                    bonus.Source = "用户(" + entity.Mobile + ")后台激活成功，获赠碳积分";
                    bonus.FromUserID = entity.Id;
                    bonus.IsSettled = 1;
                    dbc.Bonus.Add(bonus);

                    JournalEntity journal = new JournalEntity();
                    journal.UserId = entity.Id;
                    journal.BalanceAmount = entity.Amount;
                    journal.InAmount = 100;
                    journal.Remark = "用户(" + entity.Mobile + ")后台激活成功，获赠碳积分";
                    journal.JournalTypeId = (int)JournalTypeEnum.会员激活;
                    journal.OrderCode = "";
                    journal.GoodsId = 0;//来至订单ID
                    journal.CurrencyType = (int)CurrencyEnums.碳积分;
                    dbc.Journals.Add(journal);
                }
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return false;
                }
                var address = dbc.GetAll<AddressEntity>().Where(a => a.UserId == id);
                if (address.LongCount() > 0)
                {
                    await address.ForEachAsync(a => a.IsDeleted = true);
                }
                var bankAccounts = dbc.GetAll<BankAccountEntity>().Where(a => a.UserId == id);
                if (bankAccounts.LongCount() > 0)
                {
                    await bankAccounts.ForEachAsync(a => a.IsDeleted = true);
                }

                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> FrozenAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsEnabled = !entity.IsEnabled;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<long> ResetPasswordAsync(long id, string password, string newPassword)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return -1;
                }
                if (entity.Password != CommonHelper.GetMD5(password + entity.Salt))
                {
                    return -2;
                }
                entity.Password = CommonHelper.GetMD5(newPassword + entity.Salt);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> ResetPasswordAsync(long id, string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return -1;
                }
                entity.Password = CommonHelper.GetMD5(password + entity.Salt);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> ResetPasswordAsync(string mobile, string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Mobile == mobile);
                if (entity == null)
                {
                    return -1;
                }
                entity.Password = CommonHelper.GetMD5(password + entity.Salt);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> ResetTradePasswordAsync(string mobile, string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Mobile == mobile);
                if (entity == null)
                {
                    return -1;
                }
                entity.TradePassword = CommonHelper.GetMD5(password + entity.Salt);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }


        public async Task<long> UserCheck(string mobile)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                long id = await dbc.GetIdAsync<UserEntity>(u => u.Mobile == mobile);
                if (id == 0)
                {
                    return -1;
                }
                return id;
            }
        }
        public long GetUserShopUIDAsync(string mobile)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = dbc.GetAll<UserEntity>().SingleOrDefault(u => u.Mobile == mobile);
                if (entity == null)
                {
                    return -1;
                }
                if (entity.IsEnabled == false)
                {
                    return -3;
                }
                return entity.ShopUID;
            }
        }
        public async Task<long> CheckLoginAsync(string mobile,string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Mobile == mobile);
                if (entity == null)
                {
                    return -1;
                }
                if (entity.Password != CommonHelper.GetMD5(password + entity.Salt))
                {
                    return -2;
                }
                if (entity.IsEnabled == false)
                {
                    return -3;
                }
                return entity.Id;
            }
        }

        //public async Task<long> CheckTradePasswordAsync(long id, string tradePassword)
        //{
        //    using (MyDbContext dbc = new MyDbContext())
        //    {
        //        UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == id);
        //        if(user==null)
        //        {
        //            return -1;
        //        }
        //        if (user.TradePassword != CommonHelper.GetMD5(tradePassword + user.Salt))
        //        {
        //            return -2;
        //        }
        //        return 1;
        //    }
        //}
        public async Task<long> CheckTradePasswordAsync(long id, string tradePassword)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return -1;
                }
                if (user.TradePassword != "SY" + CommonHelper.GetMD5(CommonHelper.GetMD5(tradePassword)))
                {
                    return -2;
                }
                return 1;
            }
        }
        public bool CheckUserId(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                long res = dbc.GetId<UserEntity>(u => u.Id == id);
                if (res == 0)
                {
                    return false;
                }
                return true;
            }
        }

        public async Task<long> BalancePayAsync(long orderId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o => o.Id == orderId);
                if (order == null)
                    return -1;

                UserEntity user = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == order.BuyerId);
                if (user == null)
                    return -2;

                if (order.Amount > user.Amount)
                    return -4;

                //更新库存、销量
                UpdateGoodsInfo(dbc, order);

                int upLevelId = 1;
                user.Amount = user.Amount - order.Amount;

                order.PayTime = DateTime.Now;
                order.PayTypeId = await dbc.GetIdAsync<IdNameEntity>(i => i.Name == "余额");
                order.OrderStateId = await dbc.GetIdAsync<IdNameEntity>(i => i.Name == "待发货");
                if (order.Deliver == "无需物流")
                {
                    order.OrderStateId = await dbc.GetIdAsync<IdNameEntity>(i => i.Name == "已完成");
                }

                JournalEntity journal = new JournalEntity();
                journal.UserId = user.Id;
                journal.BalanceAmount = user.Amount;
                journal.OutAmount = order.Amount;
                journal.Remark = "购买商品";
                journal.JournalTypeId = 1;
                journal.OrderCode = order.Code;
                journal.LevelId = upLevelId;
                dbc.Journals.Add(journal);

                int result = await dbc.SaveChangesAsync();
                if (result > 0)
                   await CalcSroce(user.Id, order.Amount); // 累计业绩
                return 1;
            }
        }

        public long WeChatPay(string code)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity order = dbc.GetAll<OrderEntity>().SingleOrDefault(o => o.Code == code);
                if (order == null)
                    return -1;

                if (order.OrderState.Name != "待付款")
                    return -4;

                UserEntity user = dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefault(u => u.Id == order.BuyerId);
                if (user == null)
                    return -2;


                //更新库存、销量
                UpdateGoodsInfo(dbc, order);

                #region 订单商品遍历
                // var orderlists = dbc.GetAll<OrderListEntity>().Where(o => o.OrderId == order.Id).ToList();
                // decimal totalAmount = 0;


                /*
                foreach (var orderlist in orderlists)
                {
                    GoodsEntity goods = dbc.GetAll<GoodsEntity>().SingleOrDefault(g => g.Id == orderlist.GoodsId);

                    totalAmount = totalAmount + orderlist.TotalFee;

                    if (goods == null)
                    {
                        continue;
                    }

                    if (!goods.IsPutaway)
                    {
                        return -5;
                    }

                    if (goods.Inventory < orderlist.Number)
                    {
                        return -3;
                    }

                    BonusRatioEntity bonusRatio = dbc.GetAll<BonusRatioEntity>().SingleOrDefault(b => b.GoodsId == goods.Id);
                    decimal one = 0;
                    decimal two = 0;
                    decimal three = 0;

                    long journalTypeId = dbc.GetId<IdNameEntity>(i => i.Name == "佣金收入");

                    UserEntity oneer = dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefault(u => u.Id == user.Recommend.RecommendId);
                    if (oneer != null && oneer.Recommend.RecommendPath != "0")
                    {
                        one = bonusRatio.CommonOne * orderlist.Number;

                        //if (oneer.Level.Name == "普通会员" && bonusRatio != null)
                        //{
                        //    one = bonusRatio.CommonOne / 100;
                        //}
                        //else if (oneer.Level.Name == "黄金会员" && bonusRatio != null)
                        //{
                        //    one = bonusRatio.GoldOne / 100;
                        //}
                        //else if (oneer.Level.Name == "铂金会员" && bonusRatio != null)
                        //{
                        //    one = bonusRatio.PlatinumOne / 100;
                        //}

                        oneer.Amount = oneer.Amount + one;
                        oneer.BonusAmount = oneer.BonusAmount + orderlist.TotalFee * one;

                        JournalEntity journal1 = new JournalEntity();
                        journal1.UserId = oneer.Id;
                        //journal1.BalanceAmount = oneer.Amount;
                        journal1.InAmount = one;
                        journal1.Remark = "商品佣金收入";
                        journal1.JournalTypeId = journalTypeId;
                        journal1.OrderCode = order.Code;
                        journal1.GoodsId = goods.Id;
                        journal1.IsEnabled = false;
                        dbc.Journals.Add(journal1);

                        UserEntity twoer = dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefault(u => u.Id == oneer.Recommend.RecommendId);
                        if (twoer != null && twoer.Recommend.RecommendPath != "0")
                        {
                            two = bonusRatio.CommonTwo * orderlist.Number;
                            //if (twoer.Level.Name == "普通会员" && bonusRatio != null)
                            //{
                            //    two = bonusRatio.CommonTwo / 100;
                            //}
                            //else if (twoer.Level.Name == "黄金会员" && bonusRatio != null)
                            //{
                            //    two = bonusRatio.GoldTwo / 100;
                            //}
                            //else if (twoer.Level.Name == "铂金会员" && bonusRatio != null)
                            //{
                            //    two = bonusRatio.PlatinumTwo / 100;
                            //}

                            twoer.Amount = twoer.Amount + two;
                            twoer.BonusAmount = twoer.BonusAmount + two;

                            JournalEntity journal2 = new JournalEntity();
                            journal2.UserId = twoer.Id;
                            //journal2.BalanceAmount = twoer.Amount;
                            journal2.InAmount = two;
                            journal2.Remark = "商品佣金收入";
                            journal2.JournalTypeId = journalTypeId;
                            journal2.OrderCode = order.Code;
                            journal2.GoodsId = goods.Id;
                            journal2.IsEnabled = false;
                            dbc.Journals.Add(journal2);

                            UserEntity threer = dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefault(u => u.Id == twoer.Recommend.RecommendId);
                            if (threer != null && threer.Recommend.RecommendPath != "0")
                            {
                                three = bonusRatio.CommonThree * orderlist.Number;
                                //if (threer.Level.Name == "普通会员" && bonusRatio != null)
                                //{
                                //    three = bonusRatio.CommonThree / 100;
                                //}
                                //else if (threer.Level.Name == "黄金会员" && bonusRatio != null)
                                //{
                                //    three = bonusRatio.GoldThree / 100;
                                //}
                                //else if (threer.Level.Name == "铂金会员" && bonusRatio != null)
                                //{
                                //    three = bonusRatio.PlatinumThree / 100;
                                //}

                                threer.FrozenAmount = threer.FrozenAmount + three;
                                //threer.BonusAmount = threer.BonusAmount + orderlist.TotalFee * three;

                                JournalEntity journal3 = new JournalEntity();
                                journal3.UserId = threer.Id;
                                //journal3.BalanceAmount = threer.Amount;
                                journal3.InAmount = three;
                                journal3.Remark = "商品佣金收入";
                                journal3.JournalTypeId = journalTypeId;
                                journal3.OrderCode = order.Code;
                                journal3.GoodsId = goods.Id;
                                journal3.IsEnabled = false;
                                dbc.Journals.Add(journal3);
                            }
                        }
                    }
                    goods.Inventory = goods.Inventory - orderlist.Number;
                    goods.SaleNum = goods.SaleNum + orderlist.Number;
                } 
                */
                #endregion

                int upLevelId = 1;

                order.PayTime = DateTime.Now;
                order.PayTypeId = dbc.GetId<IdNameEntity>(i => i.Name == "微信");
                order.OrderStateId = dbc.GetId<IdNameEntity>(i => i.Name == "待发货");
                if (order.Deliver == "无需物流")
                    order.OrderStateId = dbc.GetId<IdNameEntity>(i => i.Name == "已完成");

                JournalEntity journal = new JournalEntity();
                journal.UserId = user.Id;
                journal.BalanceAmount = user.Amount;
                journal.OutAmount = order.Amount;
                journal.Remark = "微信支付购买商品";
                journal.JournalTypeId = 1;
                journal.LevelId = upLevelId;
                journal.OrderCode = order.Code;
                dbc.Journals.Add(journal);

                if (dbc.SaveChanges() > 0)
                    CalcSroce(user.Id, order.Amount); // 累计业绩

                log.DebugFormat("微信支付后订单状态：{0}", order.OrderStateId);
                return 1;
            }
        }

        /// <summary>
        /// 更新库存、销量
        /// </summary>
        /// <param name="dbc"></param>
        /// <param name="order"></param>
        private static void UpdateGoodsInfo(MyDbContext dbc, OrderEntity order)
        {
            var orderlists = dbc.GetAll<OrderListEntity>().Where(o => o.OrderId == order.Id).ToList();
            foreach (var orderlist in orderlists)
            {
                GoodsEntity goods = dbc.GetAll<GoodsEntity>().SingleOrDefault(g => g.Id == orderlist.GoodsId);
                goods.Inventory = goods.Inventory - orderlist.Number;
                goods.SaleNum = goods.SaleNum + orderlist.Number;
            }
        }
        /// <summary>
        /// 计算奖金
        /// </summary>
        /// <param name="dbc"></param>
        /// <param name="order"></param>
        /// <param name="user"></param>


        public async Task<CalcAmountResult> CalcCount()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                CalcAmountResult res = new CalcAmountResult();
                var users = dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.IsNull == false);
                var takeCash = dbc.GetAll<TakeCashEntity>().AsNoTracking().Where(t => t.State.Name == "已结款");
                res.TotalAmount = users.Any() ? await users.SumAsync(u => u.Amount) : 0;
                res.TotalTakeCash = takeCash.Any() ? await takeCash.SumAsync(u => u.Amount) : 0;
                res.TotalBuyAmount = users.Any() ? await users.SumAsync(u => u.BuyAmount) : 0;
                return res;
            }
        }

        public UserDTO GetModel(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = dbc.GetAll<UserEntity>().AsNoTracking().SingleOrDefault(u => u.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }


        public async Task<UserDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().AsNoTracking().SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<long> GetUserRecommendIdAysnc(long userId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                long res = await dbc.GetAll<UserEntity>().Where(u => u.Id == userId).Select(u=>u.RecommendId).SingleOrDefaultAsync();
                return res;
            }
        }

        public async Task<string> GetMobileByIdAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                string mobile = await dbc.GetParameterAsync<UserEntity>(u => u.Id == id, u => u.Mobile);
                if (mobile == null)
                {
                    return "";
                }
                return mobile;
            }
        }

        public async Task<string> GetUserCodeByIdAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                string mobile = await dbc.GetParameterAsync<UserEntity>(u => u.Id == id, u => u.UserCode);
                if (mobile == null)
                {
                    return "";
                }
                return mobile;
            }
        }

        public async Task<long> GetIdByMobile(string mobile)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                long id = await dbc.GetIdAsync<UserEntity>(u => u.Mobile == mobile);

                return id;
            }
        }
        public async Task<UserDTO> GetModelByMobileAsync(string mobile)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserSearchResult result = new UserSearchResult();
                var user = await dbc.GetAll<UserEntity>().AsNoTracking().FirstOrDefaultAsync(u => u.Mobile == mobile);
                if (user == null)
                {
                    return null;
                }
                return ToDTO(user);
            }
        }
        public async Task<UserDTO> GetModelByUserCodeAsync(string userCode)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var user = await dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.UserCode == userCode);
                if (user == null)
                {
                    return null;
                }
                return ToDTO(user);
            }
        }
        public async Task<UserSearchResult> GetModelListAsync(int? levelId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserSearchResult result = new UserSearchResult();
                var users = dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.IsNull == false);

                if (levelId != null)
                {
                    users = users.Where(a => a.LevelId == levelId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    users = users.Where(a => a.Mobile.Contains(keyword) || a.Code.Contains(keyword) || a.NickName.Contains(keyword) || a.UserCode.Contains(keyword));
                }
                if (startTime != null)
                {
                    users = users.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    users = users.Where(a => SqlFunctions.DateDiff("day", endTime, a.CreateTime) <= 0);
                }
                result.PageCount = (int)Math.Ceiling((await users.LongCountAsync()) * 1.0f / pageSize);
                var userResult = await users.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Users = userResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }
        //public async Task<UserTeamSearchResult> GetModelTeamListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        //{
        //    using (MyDbContext dbc = new MyDbContext())
        //    {
        //        UserRegistSearchResult result = new UserRegistSearchResult();

        //        var lists = dbc.GetAll<RegisterEntity>().AsNoTracking().Where(s=>s.Flag == 1);

        //        if (!string.IsNullOrEmpty(keyword))
        //        {
        //            lists = lists.Where(a => a.User.NickName.Contains(keyword) || a.User.UserCode.Contains(keyword) || a.Profession.Contains(keyword)
        //            || a.SchoolName.Contains(keyword) || a.TeamName.Contains(keyword) || a.QQ.Contains(keyword));
        //        }
        //        if (startTime != null)
        //        {
        //            lists = lists.Where(a => a.FlagTime >= startTime);
        //        }
        //        if (endTime != null)
        //        {
        //            lists = lists.Where(a => SqlFunctions.DateDiff("day", endTime, a.FlagTime) <= 0);
        //        }
        //        result.TotalCount = lists.LongCount();
        //        result.PageCount = (int)Math.Ceiling(result.TotalCount * 1.0f / pageSize);

        //        var userResult = await lists.Include(u => u.User).OrderByDescending(a => a.FlagTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        //        result.Register = userResult.ToArray();
        //        return result;
        //    }
        //}
        /// <summary>
        /// 累计业绩
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sroce"></param>
        /// <returns></returns>
        public async Task<bool> CalcSroce(long userId, decimal sroce)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                int result = await dbc.Database.ExecuteSqlCommandAsync("exec proc_Sroce @UserId = " + userId + ",@sroce = " + sroce);
                return true;
            }
        }
        /// <summary>
        /// 管理级别升级
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sroce"></param>
        /// <returns></returns>
        public async Task<bool> ManageLevelUp()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                int result = await dbc.Database.ExecuteSqlCommandAsync("exec proc_manageLevelUp ");
                return true;
            }
        }
        public async Task<int> AccountChange(string mobile, int type, decimal amount, string remark)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var price = await cashHallService.GetExchangeAsync();
                decimal aAamount = amount / price; //换算为 A积分

                var user = await dbc.GetAll<UserEntity>().FirstOrDefaultAsync(u => u.Mobile == mobile);
                if (user == null)
                {
                    return -1;
                }
                if (type == 1)
                {
                    user.BonusAmount += aAamount;
                    //type :1-加，0-减
                    int JournalTypeId = 1;
                    await journalService.AddAsync(user.Id, aAamount, JournalTypeId, (int)CurrencyEnums.碳积分, user.BonusAmount, remark, 1);

                }
                else
                {
                    if (user.BonusAmount < aAamount)
                    {
                        return -2;
                    }
                    user.BonusAmount -= aAamount;

                    //type :1-加，0-减
                    int JournalTypeId = 1;
                    await journalService.AddAsync(user.Id, aAamount, JournalTypeId, (int)CurrencyEnums.碳积分, user.BonusAmount, remark, 0);

                }
                int result = await dbc.SaveChangesAsync();
                return result;
            }
        }

        public async Task<UserRecommendResult> GetFirstRecommendListAsync(string mobile, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserRecommendResult result = new UserRecommendResult();
                long id = await dbc.GetIdAsync<UserEntity>(u => u.Mobile == mobile);
                if (id == 0)
                {
                    id = -1;
                }
                var users = dbc.GetAll<UserEntity>().Where(u => u.RecommendId == id);
                result.PageCount = (int)Math.Ceiling((await users.LongCountAsync()) * 1.0f / pageSize);
                var userResult = await users.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.UserRecommends = userResult.Select(a => new UserRecommendDTO { User_Name = a.Mobile, TeamScore = a.TeamScore }).ToArray();
                return result;
            }
        }
        /// <summary>
        /// 查询会员极差等级
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public async Task<string> SearchMlevelNameAsync(string mobile)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                int mLevelId = await dbc.GetAll<UserEntity>().Where(u=>u.Mobile==mobile).Select(u => u.MLevelId).SingleOrDefaultAsync();
                return mLevelId.GetEnumName<MLevelEnum>();
            }
        }

        public async Task<long> SetLevelUpAsync(string mobile, int levelId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                //一级会员：1，二级会员：2，三级会员：3，四级会员：4
                if (levelId <= 0 || levelId > 4)
                {
                    return -1;
                }
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Mobile == mobile);
                if(user==null)
                {
                    return -2;
                }
                user.LevelId = levelId;
                await dbc.SaveChangesAsync();
                return user.Id;
            }
        }

        /// <summary>
        /// 极差级别升级
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="levelId">一档：1，二档：2，三档：3，四档：4，五档：5</param>
        /// <returns></returns>
        public async Task<int> SetMLevelUpAsync(string mobile, int levelId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                //一档：1，二档：2，三档：3，四档：4，五档：5
                if(levelId<=0 || levelId>5)
                {
                    return -1;
                }
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u=>u.Mobile==mobile);
                if(levelId<user.MLevelId)
                {
                    return -2;
                }
                user.MLevelId = levelId;
                await dbc.SaveChangesAsync();
                return 1;
            }
        }

        public async Task<UserRecommendTreeDTO[]> GetRecommendListAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var users = dbc.GetAll<UserEntity>().Where(u => u.RecommendId == id);
                var res = await users.ToListAsync();
                return res.Select(u=>ToDTO(u,u.MLevelId)).ToArray();
            }
        }

        public async Task<UserSearchResult> GetActivateListAsync(int pageIndex,int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserSearchResult result = new UserSearchResult();
                var users = dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.IsUpgraded == false);
                
                result.PageCount = (int)Math.Ceiling((await users.LongCountAsync()) * 1.0f / pageSize);
                var userResult = await users.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Users = userResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<UserRecommendTreeDTO> GetModelTreeAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var user = await dbc.GetAll<UserEntity>().AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return null;
                }
                return ToDTO(user, user.MLevelId);
            }
        }

        public async Task<UserRecommendTreeDTO> GetModelTreeByMobileAsync(string mobile)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var user = await dbc.GetAll<UserEntity>().AsNoTracking().FirstOrDefaultAsync(u => u.Mobile == mobile);
                if (user == null)
                {
                    return null;
                }
                return ToDTO(user,user.MLevelId);
            }
        }
    }
}
