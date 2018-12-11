using IMS.Common;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class CourseOrderController : Controller
    {
        public ICourseOrderService courseOrderService { get; set; }
        private int pageSize = 10;

        #region 课程订单列表
        [HttpGet]
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        [AdminLog("打款管理", "查看打款管理列表")]
        public async Task<ActionResult> List(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var res = await courseOrderService.GetModelListAsync(keyword,startTime,endTime,pageIndex,pageSize);
            return Json(new AjaxResult { Status = 1, Data = res });
        }
        #endregion

        #region 确认打款
        [HttpPost]
        [AdminLog("打款管理", "确认打款")]
        [Permission("打款管理_确认打款")]
        public async Task<ActionResult> Audit(long id,int stateId)
        {
            bool res = await courseOrderService.AuditAsync(id,stateId, Convert.ToInt64(Session["Platform_AdminUserId"]));
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "审核激活会员操作失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "审核激活会员操作成功", Data = "/admin/courseOrder/list" });
        }
        #endregion
    }
}