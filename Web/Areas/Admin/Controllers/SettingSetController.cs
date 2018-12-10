using IMS.Common;
using IMS.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class SettingSetController : Controller
    {
        public ISettingService settingService { get; set; }
        [HttpGet]
        public ActionResult List()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> List(bool flag = true)
        {
            var model = await settingService.GetAllIsEnableAsync();
            return Json(new AjaxResult { Status = 1, Data = model });
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(long id,string parm)
        {
            bool flag = await settingService.UpdateAsync(id,parm);
            if (!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "更新失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "更新成功" });
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EditAll(SettingParm[] settings)
        {
            bool flag = await settingService.UpdateAsync(settings);
            if(!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "更新失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "更新成功" });
        }
    }
}