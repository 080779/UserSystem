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
    public class CashOrderService : ICashOrderService
    {
        public CashOrderDTO ToDTO(CashOrderEntity entity)
        {
            CashOrderDTO dto = new CashOrderDTO();
            dto.BuyId = entity.BuyId;
            dto.SellId = entity.SellId;
            dto.BuyUserId = entity.BuyUserId;
            dto.SellUserId = entity.SellUserId;
            dto.BuyerCode = entity.BuyOrder.Buyer.Mobile;
            dto.SellerCode = entity.SellOrder.Seller.Mobile;
            dto.OrderCode = entity.OrderCode;
            dto.Number = entity.Number;
            dto.Price = entity.Price;
            dto.Amount = entity.Amount;
            dto.PayStateType = entity.PayStateType;
            dto.PayStateName = entity.PayStateType.GetEnumName<CashOrderPayStateEnums>();
            dto.PayTime = entity.PayTime;
            dto.ConfirmStateType = entity.ConfirmStateType;
            dto.ConfirmStateName = entity.ConfirmStateType.GetEnumName<CashOrderConfirmStateEnums>();
            dto.ConfirmTime = entity.ConfirmTime;
            dto.StateType = entity.StateType;
            dto.StateName = entity.StateType.GetEnumName<CashOrderStateEnums>();

            return dto;
        }
        public async Task<long> AddAsync(long buyId, long sellId, long buyUserId,long sellUserId,string orderCode, int number,decimal price, 
            decimal amount, int payStateType, int confirmStateType, int stateType)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                CashOrderEntity log = new CashOrderEntity();
                log.BuyId = buyId;
                log.SellId = sellId;
                log.BuyUserId = buyUserId;
                log.SellUserId = sellUserId;
                log.OrderCode = orderCode;
                log.Number = number;
                log.Price = price;
                log.Amount = amount;
                log.PayStateType = payStateType;
                log.ConfirmStateType = confirmStateType;
                log.StateType = stateType;
                dbc.CashOrders.Add(log);
                await dbc.SaveChangesAsync();
                return log.Id;
            }
        }

        public async Task<CashOrderSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                CashOrderSearchResult result = new CashOrderSearchResult();
                var logs = dbc.GetAll<CashOrderEntity>().AsNoTracking();
                if (!string.IsNullOrEmpty(keyword))
                {
                    logs = logs.Where(a => a.OrderCode.Contains(keyword) 
                    || a.BuyOrder.Buyer.Mobile.Contains(keyword) 
                    || a.SellOrder.Seller.Mobile.Contains(keyword));
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
        //订单详情

        public async Task<CashOrderDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<CashOrderEntity>().AsNoTracking().SingleOrDefaultAsync(a => a.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }
        //平台收支记录
        public async Task<CashOrderSearchResult> GetPaymentsListAsync(long? orderId, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                CashOrderSearchResult result = new CashOrderSearchResult();
                var logs = dbc.GetAll<CashOrderEntity>().AsNoTracking();


                if (orderId != null && orderId > 0)
                {
                    logs = logs.Where(a => a.Id == orderId);
                }
                result.PageCount = (int)Math.Ceiling((await logs.LongCountAsync()) * 1.0f / pageSize);
                var logsResult = await logs.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.log = logsResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }
    }
}
