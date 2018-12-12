using IMS.Common;
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
    public class CourseOrderService : ICourseOrderService
    {
        public CourseOrderDTO ToDTO(CourseOrderEntity entity)
        {
            CourseOrderDTO dto = new CourseOrderDTO();
            dto.Id = entity.Id;
            dto.Amount = entity.Amount;
            dto.AuditMobile = entity.AuditMobile;
            dto.AuditTime = entity.AuditTime;
            dto.BuyerId = entity.BuyerId;
            dto.BuyerName = entity.BuyerName;
            dto.Code = entity.Code;
            dto.CourseId = entity.CourseId;
            dto.CourseName = entity.CourseName;
            dto.CreateTime = entity.CreateTime;
            dto.DiscountAmount = entity.DiscountAmount;
            dto.ImgUrl = entity.ImgUrl;
            dto.OrderStateName = entity.OrderStateId.GetEnumName<CourseOrderStateEnum>();
            return dto;
        }
        public async Task<long> AddAsync(long buyerId, string buyerName, long courseId, string imgUrl)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                int res = await dbc.GetAll<CourseOrderEntity>().Where(c => c.CourseId == courseId && c.BuyerId==buyerId && c.OrderStateId!=(int)CourseOrderStateEnum.已取消).Select(c => c.OrderStateId).SingleOrDefaultAsync();
                //订单已经存在且状态不是已取消
                if (res > 0)
                {
                    return -1;
                }
                CourseOrderEntity entity = new CourseOrderEntity();
                entity.Code= CommonHelper.GetRandom3();
                entity.BuyerId = buyerId;
                entity.BuyerName = buyerName;
                entity.CourseId = courseId;
                entity.PayTypeId = 1;
                entity.CourseName = await dbc.GetParameterAsync<LinkEntity>(l=>l.Id==courseId,l=>l.Name);
                entity.Amount = await dbc.GetDecimalParameterAsync<LinkEntity>(l => l.Id == courseId, l => l.Link001);
                entity.ImgUrl = imgUrl;
                dbc.CourseOrders.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> AddAsync(long buyerId, string buyerName, long courseId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                int res = await dbc.GetAll<CourseOrderEntity>().Where(c => c.CourseId == courseId && c.BuyerId == buyerId && c.OrderStateId != (int)CourseOrderStateEnum.已取消 && c.PayTypeId==2).Select(c => c.OrderStateId).SingleOrDefaultAsync();
                //订单已经存在且状态不是已取消
                if (res > 0)
                {
                    return -1;
                }
                decimal amount= await dbc.GetAll<LinkEntity>().Where(l => l.Id == courseId).Select(l => l.link002).SingleOrDefaultAsync();
                var user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == buyerId);
                if(user.Amount<amount)
                {
                    return -2;
                }
                user.Amount = user.Amount - amount;

                
                CourseOrderEntity entity = new CourseOrderEntity();
                entity.Code = CommonHelper.GetRandom3();
                entity.BuyerId = buyerId;
                entity.BuyerName = buyerName;
                entity.CourseId = courseId;
                entity.PayTypeId = 2;
                entity.OrderStateId = 2;
                entity.CourseName = await dbc.GetParameterAsync<LinkEntity>(l => l.Id == courseId, l => l.Name);
                entity.Amount = amount;
                entity.ImgUrl = "";
                dbc.CourseOrders.Add(entity);
                await dbc.SaveChangesAsync();

                JournalEntity journal = new JournalEntity();
                journal.UserId = user.Id;
                journal.BalanceAmount = user.Amount;
                journal.OutAmount = amount;
                journal.Remark = "用户(" + user.Mobile + ")用碳积分购买课程";
                journal.JournalTypeId = (int)JournalTypeEnum.购买课程;
                journal.OrderCode = entity.Code;
                journal.GoodsId = entity.CourseId;//来至订单ID
                journal.CurrencyType = (int)CurrencyEnums.碳积分;
                dbc.Journals.Add(journal);
                await dbc.SaveChangesAsync();

                //积分购买课程发放奖励
                await IntegralBuyAsync(entity.Id);
                return entity.Id;
            }
        }

        public async Task<bool> AuditAsync(long id, int stateId, long auditorId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var course = await dbc.GetAll<CourseOrderEntity>().SingleOrDefaultAsync(c => c.Id == id);
                if (course == null)
                {
                    return false;
                }
                //取消课程订单
                if((int)CourseOrderStateEnum.已取消 == stateId)
                {
                    course.OrderStateId = stateId;
                    course.AuditTime = DateTime.Now;
                    course.AuditMobile = await dbc.GetParameterAsync<AdminEntity>(a => a.Id == auditorId, a => a.Mobile);
                    await dbc.SaveChangesAsync();
                    return true;
                }
                else
                {
                    course.OrderStateId = stateId;
                    course.AuditTime = DateTime.Now;
                    course.AuditMobile = await dbc.GetParameterAsync<AdminEntity>(a => a.Id == auditorId, a => a.Mobile);
                                        
                    UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u=>u.Id==course.BuyerId);
                    //未激活会员
                    if(!user.IsUpgraded)
                    {
                        course.OrderStateId = (int)CourseOrderStateEnum.已取消;
                        await dbc.SaveChangesAsync();
                        return true;
                    }

                    if(user.LevelId != (int)LevelEnum.普通会员)
                    {
                        //购买课程成功，获得100碳积分
                        user.Amount = user.Amount + 100;                        
                    }
                    else
                    {
                        //购买课程成功，会员升级为节点会员,获得100碳积分
                        user.LevelId = (int)LevelEnum.创客会员;
                        user.Amount = user.Amount + 100;
                    }
                    
                    BonusEntity entity = new BonusEntity();
                    entity.UserId = user.Id;
                    entity.Amount = 100;
                    entity.Revenue = 0;
                    entity.sf = 100;
                    entity.TypeID = 1; //购买课程奖励
                    entity.Source = "用户(" + user.Mobile + ")购买课程,获得碳积分奖励";
                    entity.FromUserID = user.Id;
                    entity.IsSettled = 1;
                    dbc.Bonus.Add(entity);

                    JournalEntity journal = new JournalEntity();
                    journal.UserId = user.Id;
                    journal.BalanceAmount = user.Amount;
                    journal.InAmount = 100;
                    journal.Remark = "用户(" + user.Mobile + ")购买课程,获得碳积分奖励";
                    journal.JournalTypeId = (int)JournalTypeEnum.购买课程奖励;
                    journal.OrderCode = course.Code;
                    journal.GoodsId = course.CourseId;//来至订单ID
                    journal.CurrencyType = (int)CurrencyEnums.碳积分;
                    dbc.Journals.Add(journal);

                    if (user.RecommendId <= 0)
                    {
                        await dbc.SaveChangesAsync();
                        return true;
                    }

                    UserEntity recUser = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == user.RecommendId);
                    if (recUser.LevelId == (int)LevelEnum.普通会员)
                    {
                        await dbc.SaveChangesAsync();
                        return true;
                    }

                    //推荐奖
                    decimal param= (await dbc.GetDecimalParamAsync("直接推荐奖励比例")) / 100;
                    decimal blance = 100 * param;
                    recUser.Amount = recUser.Amount + blance;

                    BonusEntity entity1 = new BonusEntity();
                    entity1.UserId = recUser.Id;
                    entity1.Amount = blance;
                    entity1.Revenue = 0;
                    entity1.sf = blance;
                    entity1.TypeID = 2; //直接推荐奖
                    entity1.Source = "用户(" + user.Mobile + ")购买课程,用户(" + recUser.Mobile + ")获得直接推荐奖励";
                    entity1.FromUserID = user.Id;
                    entity1.IsSettled = 1;
                    dbc.Bonus.Add(entity1);

                    JournalEntity journal1 = new JournalEntity();
                    journal1.UserId = recUser.Id;
                    journal1.BalanceAmount = recUser.Amount;
                    journal1.InAmount = blance;
                    journal1.Remark = "用户(" + user.Mobile + ")购买课程,用户(" + recUser.Mobile + ")获得直接推荐奖励";
                    journal1.JournalTypeId = (int)JournalTypeEnum.直推奖;
                    journal1.OrderCode = course.Code;
                    journal1.GoodsId = course.CourseId;//来至订单ID
                    journal1.CurrencyType = (int)CurrencyEnums.碳积分;
                    dbc.Journals.Add(journal1);

                    long recommendId = recUser.RecommendId;
                    int count = 0;
                    long recCount = await dbc.GetAll<UserEntity>().AsNoTracking().LongCountAsync(u=>u.RecommendId==recUser.Id && u.LevelId== (int)LevelEnum.创客会员);
                    if(recCount >= 10)
                    {
                        recUser.LevelId = (int)LevelEnum.贵宾会员;
                        count ++;
                    }

                    decimal param1 = (await dbc.GetDecimalParamAsync("招募创客6到9个奖金比例")) / 100;
                    decimal param2 = (await dbc.GetDecimalParamAsync("招募创客大于9个奖金比例")) / 100;
                    //领导奖
                    while (recommendId > 0 && recCount >= 6)
                    {
                        recUser= await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == recommendId);
                        recCount = await dbc.GetAll<UserEntity>().AsNoTracking()
                            .LongCountAsync(u => u.RecommendId == recUser.Id && (u.LevelId == (int)LevelEnum.贵宾会员 || u.LevelId==(int)LevelEnum.超级会员));
                        recommendId = recUser.RecommendId;

                        if(recCount < 6)
                        {
                            break;
                        }
                        else if(recCount >= 6 & recCount < 10)
                        {
                            blance = blance * 60 / 100;
                            recUser.Amount = recUser.Amount + blance;
                            count = 0;

                            BonusEntity entity2 = new BonusEntity();
                            entity2.UserId = recUser.Id;
                            entity2.Amount = blance;
                            entity2.Revenue = 0;
                            entity2.sf = blance;
                            entity2.TypeID = 3; //领导奖
                            entity2.Source = "用户(" + user.Mobile + ")购买课程,用户(" + recUser.Mobile + ")获得领导奖励";
                            entity2.FromUserID = user.Id;
                            entity2.IsSettled = 1;
                            dbc.Bonus.Add(entity2);

                            JournalEntity journal2 = new JournalEntity();
                            journal2.UserId = recUser.Id;
                            journal2.BalanceAmount = recUser.Amount;
                            journal2.InAmount = blance;
                            journal2.Remark = "用户(" + user.Mobile + ")购买课程,用户(" + recUser.Mobile + ")获得领导奖励";
                            journal2.JournalTypeId = (int)JournalTypeEnum.领导奖;
                            journal2.OrderCode = course.Code;
                            journal2.GoodsId = course.CourseId;//来至订单ID
                            journal2.CurrencyType = (int)CurrencyEnums.碳积分;
                            dbc.Journals.Add(journal1);
                        }
                        else
                        {
                            blance = blance * 80 / 100;
                            recUser.Amount = recUser.Amount + blance;
                            count++;
                            if (count >= 10 && recUser.LevelId == (int)LevelEnum.贵宾会员)
                            {
                                recUser.LevelId = (int)LevelEnum.超级会员;
                            }

                            BonusEntity entity3 = new BonusEntity();
                            entity3.UserId = recUser.Id;
                            entity3.Amount = blance;
                            entity3.Revenue = 0;
                            entity3.sf = blance;
                            entity3.TypeID = 3; //领导奖
                            entity3.Source = "用户(" + user.Mobile + ")购买课程,用户(" + recUser.Mobile + ")获得领导奖励";
                            entity3.FromUserID = user.Id;
                            entity3.IsSettled = 1;
                            dbc.Bonus.Add(entity3);

                            JournalEntity journal3 = new JournalEntity();
                            journal3.UserId = recUser.Id;
                            journal3.BalanceAmount = recUser.Amount;
                            journal3.InAmount = blance;
                            journal3.Remark = "用户(" + user.Mobile + ")购买课程,用户(" + recUser.Mobile + ")获得领导奖励";
                            journal3.JournalTypeId = (int)JournalTypeEnum.领导奖;
                            journal3.OrderCode = course.Code;
                            journal3.GoodsId = course.CourseId;//来至订单ID
                            journal3.CurrencyType = (int)CurrencyEnums.碳积分;
                            dbc.Journals.Add(journal3);
                        }                        
                    }

                    await dbc.SaveChangesAsync();
                    return true;
                }
            }
        }

        /// <summary>
        /// 积分购买课程发放奖励
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> IntegralBuyAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var course = await dbc.GetAll<CourseOrderEntity>().SingleOrDefaultAsync(c => c.Id == id);
                if (course == null)
                {
                    return false;
                }
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == course.BuyerId);
                //未激活会员
                if (!user.IsUpgraded)
                {
                    return true;
                }
                //不是普通会员，无法升级
                if (user.LevelId != (int)LevelEnum.普通会员)
                {
                    return true;
                }
                else
                {
                    //购买课程成功，会员升级为节点会员
                    user.LevelId = (int)LevelEnum.创客会员;
                    await dbc.SaveChangesAsync();
                }

                if (user.RecommendId <= 0)
                {
                    return true;
                }

                UserEntity recUser = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == user.RecommendId);
                if(!recUser.IsUpgraded)
                {
                    return true;
                }

                if (recUser.LevelId == (int)LevelEnum.普通会员)
                {
                    return true;
                }

                //推荐奖
                decimal param = (await dbc.GetDecimalParamAsync("直接推荐奖励比例")) / 100;
                decimal blance = 100 * param;
                recUser.Amount = recUser.Amount + blance;

                BonusEntity entity1 = new BonusEntity();
                entity1.UserId = recUser.Id;
                entity1.Amount = blance;
                entity1.Revenue = 0;
                entity1.sf = blance;
                entity1.TypeID = 2; //直接推荐奖
                entity1.Source = "用户(" + user.Mobile + ")购买课程,用户(" + recUser.Mobile + ")获得直接推荐奖励";
                entity1.FromUserID = user.Id;
                entity1.IsSettled = 1;
                dbc.Bonus.Add(entity1);

                JournalEntity journal1 = new JournalEntity();
                journal1.UserId = recUser.Id;
                journal1.BalanceAmount = recUser.Amount;
                journal1.InAmount = blance;
                journal1.Remark = "用户(" + user.Mobile + ")购买课程,用户(" + recUser.Mobile + ")获得直接推荐奖励";
                journal1.JournalTypeId = (int)JournalTypeEnum.直推奖;
                journal1.OrderCode = course.Code;
                journal1.GoodsId = course.CourseId;//来至订单ID
                journal1.CurrencyType = (int)CurrencyEnums.碳积分;
                dbc.Journals.Add(journal1);

                long recommendId = recUser.RecommendId;
                int count = 0;
                long recCount = await dbc.GetAll<UserEntity>().AsNoTracking().LongCountAsync(u => u.RecommendId == recUser.Id && u.LevelId == (int)LevelEnum.创客会员);
                if (recCount >= 10)
                {
                    recUser.LevelId = (int)LevelEnum.贵宾会员;
                    count++;
                }

                decimal param1 = (await dbc.GetDecimalParamAsync("招募创客6到9个奖金比例")) / 100;
                decimal param2 = (await dbc.GetDecimalParamAsync("招募创客大于9个奖金比例")) / 100;
                //领导奖
                while (recommendId > 0 && recCount >= 6)
                {
                    recUser = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == recommendId);
                    recCount = await dbc.GetAll<UserEntity>().AsNoTracking()
                        .LongCountAsync(u => u.RecommendId == recUser.Id && (u.LevelId == (int)LevelEnum.贵宾会员 || u.LevelId == (int)LevelEnum.超级会员));
                    recommendId = recUser.RecommendId;

                    if (recCount < 6)
                    {
                        break;
                    }
                    else if (recCount >= 6 & recCount < 10)
                    {
                        blance = blance * param1 / 100;
                        recUser.Amount = recUser.Amount + blance;
                        count = 0;

                        BonusEntity entity2 = new BonusEntity();
                        entity2.UserId = recUser.Id;
                        entity2.Amount = blance;
                        entity2.Revenue = 0;
                        entity2.sf = blance;
                        entity2.TypeID = 3; //领导奖
                        entity2.Source = "用户(" + user.Mobile + ")购买课程,用户(" + recUser.Mobile + ")获得领导奖励";
                        entity2.FromUserID = user.Id;
                        entity2.IsSettled = 1;
                        dbc.Bonus.Add(entity2);

                        JournalEntity journal2 = new JournalEntity();
                        journal2.UserId = recUser.Id;
                        journal2.BalanceAmount = recUser.Amount;
                        journal2.InAmount = blance;
                        journal2.Remark = "用户(" + user.Mobile + ")购买课程,用户(" + recUser.Mobile + ")获得领导奖励";
                        journal2.JournalTypeId = (int)JournalTypeEnum.领导奖;
                        journal2.OrderCode = course.Code;
                        journal2.GoodsId = course.CourseId;//来至订单ID
                        journal2.CurrencyType = (int)CurrencyEnums.碳积分;
                        dbc.Journals.Add(journal1);
                    }
                    else
                    {
                        blance = blance * param2 / 100;
                        recUser.Amount = recUser.Amount + blance;
                        count++;
                        if (count >= 10 && recUser.LevelId == (int)LevelEnum.贵宾会员)
                        {
                            recUser.LevelId = (int)LevelEnum.超级会员;
                        }

                        BonusEntity entity3 = new BonusEntity();
                        entity3.UserId = recUser.Id;
                        entity3.Amount = blance;
                        entity3.Revenue = 0;
                        entity3.sf = blance;
                        entity3.TypeID = 3; //领导奖
                        entity3.Source = "用户(" + user.Mobile + ")购买课程,用户(" + recUser.Mobile + ")获得领导奖励";
                        entity3.FromUserID = user.Id;
                        entity3.IsSettled = 1;
                        dbc.Bonus.Add(entity3);

                        JournalEntity journal3 = new JournalEntity();
                        journal3.UserId = recUser.Id;
                        journal3.BalanceAmount = recUser.Amount;
                        journal3.InAmount = blance;
                        journal3.Remark = "用户(" + user.Mobile + ")购买课程,用户(" + recUser.Mobile + ")获得领导奖励";
                        journal3.JournalTypeId = (int)JournalTypeEnum.领导奖;
                        journal3.OrderCode = course.Code;
                        journal3.GoodsId = course.CourseId;//来至订单ID
                        journal3.CurrencyType = (int)CurrencyEnums.碳积分;
                        dbc.Journals.Add(journal3);
                    }
                }

                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<CourseOrderSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                CourseOrderSearchResult result = new CourseOrderSearchResult();
                var courseOrders = dbc.GetAll<CourseOrderEntity>().AsNoTracking();
                if (!string.IsNullOrEmpty(keyword))
                {
                    courseOrders = courseOrders.Where(a => a.CourseName.Contains(keyword));
                }
                if (startTime != null)
                {
                    courseOrders = courseOrders.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    courseOrders = courseOrders.Where(a => SqlFunctions.DateDiff("day", endTime, a.CreateTime) <= 0);
                }
                result.PageCount = (int)Math.Ceiling((await courseOrders.LongCountAsync()) * 1.0f / pageSize);
                var courseOrderResult = await courseOrders.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.CourseOrders = courseOrderResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }
    }
}
