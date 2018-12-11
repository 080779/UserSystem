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
    public class BonusController : Controller
    {
        public IBonusService bonusService { get; set; }
        private int pageSize = 10;

        #region 奖金列表
        [HttpGet]
        //[Permission("积分奖励_积分奖励记录")]
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        [AdminLog("积分奖励", "查看积分奖励记录")]
        public async Task<ActionResult> List(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var res = await bonusService.GetModelListAsync(null,keyword,startTime,endTime,pageIndex,pageSize);
            return Json(new AjaxResult { Status = 1, Data = res });
        }
        #endregion
    }
}