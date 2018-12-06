using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.Register;
using IMS.Web.Areas.Admin.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace IMS.Web.Areas.Admin.Controllers
{
    public class RegisterController : Controller
    {
        private int pageSize = 10;
        public IRegisterService registerService { get; set; }
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        //[AdminLog("报名列表", "报名列表")]
        public async Task<ActionResult> List(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            //await orderService.AutoConfirmAsync();
            //long levelId = await idNameService.GetIdByNameAsync("会员等级");
            var result = await registerService.GetModelListAsync(keyword, startTime, endTime, pageIndex, pageSize);

            RegisterListViewModel model = new RegisterListViewModel();
            model.register = result.Register;
            model.PageCount = result.PageCount;
            return Json(new AjaxResult { Status = 1, Data = model });
        }

        [HttpPost]
        //[AdminLog("报名列表", "报名列表")]
        public async Task<ActionResult> Audit(long id )
        {
            var res = await registerService.Audit(id);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "审核失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "审核成功" });
        }
        [HttpPost]
        //[AdminLog("报名列表", "报名列表")]
        public async Task<ActionResult> Cancel(long id)
        {
            var res = await registerService.Cancel(id);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "取消失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "取消成功" });
        }
    }
}