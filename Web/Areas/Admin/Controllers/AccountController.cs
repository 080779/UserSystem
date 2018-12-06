using IMS.Common;
using IMS.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IMS.Web.Areas.Admin.Models.Order;
using IMS.DTO;
using SDMS.Common;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.Recharge;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        public IRechargeService rechargeService { get; set; }
        public IUserService userService { get; set; }
        private int pageSize = 10;
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        //[Permission("会员充值")]
        public async Task<ActionResult> Add(string usercode, int currencyType, int rechargeType, string  money)
        {
            var user = await userService.GetModelByMobileAsync(usercode);
            if (user == null)
            {
                return Json(new AjaxResult { Status = 0, Msg = "手机号不存在" });
            }

            if (Convert.ToInt32(currencyType) == 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "请选择币种类型" });
            }
            if (Convert.ToInt32(rechargeType) == 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "请选择充值类型" });
            }
            if (string.IsNullOrEmpty(money))
            {
                return Json(new AjaxResult { Status = 0, Msg = "充值金额不能为空" });
            }
            else if (Convert.ToDecimal(money) <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "充值金额需大于零" });
            }
            decimal reMoney = Convert.ToDecimal(money);

            long account = await rechargeService.AddAsync(user.Id, currencyType, rechargeType, reMoney);
            if (account<= 0)
            { 
                return Json(new AjaxResult { Status = 0, Msg = "充值失败" });
            }
                return Json(new AjaxResult { Status = 1, Msg = "充值成功" });
        }
        [HttpPost]
        //[Permission("充值列表")]
        public async Task<ActionResult> List(long? userId, string userCode, int rechargtateId, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
           
            var result = await rechargeService.GetRechargeListAsync(userId, userCode, rechargtateId, startTime, endTime, pageIndex, pageSize);
            RechargeListViewModel model = new RechargeListViewModel();
            model.Recharge = result.Recharges;
            model.PageCount = result.PageCount;
          
            return Json(new AjaxResult { Status = 1, Data = model });
        }
    }
}