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
    public class CourseController : Controller
    {
        public ILinkService linkService { get; set; }
        private int pageSize = 10;

        #region 课程列表
        [HttpGet]
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        [AdminLog("课程管理", "查看课程管理列表")]
        public async Task<ActionResult> List(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var res = await linkService.GetModelListAsync("培训课程",keyword,startTime,endTime,pageIndex,pageSize);
            return Json(new AjaxResult { Status = 1, Data = res });
        }
        #endregion

        #region 添加课程
        [HttpPost]
        [Permission("课程管理_新增课程")]
        [AdminLog("课程管理", "新增课程")]
        public async Task<ActionResult> Add(string name, int amount)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new AjaxResult { Status = 0, Msg = "课程名不能为空" });
            }
            //var res = await ImageHelper.Base64SaveAsync(imgUrl);
            //if(!res.Key)
            //{
            //    return Json(new AjaxResult { Status = 0, Msg = res.Value });
            //}
            long id = await linkService.AddAsync(2,"培训课程", name, null, null,0,amount);
            if (id <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "添加课程失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "添加课程成功", Data = "/admin/link/list" });
        }
        #endregion

        #region 修改课程
        [HttpPost]
        [Permission("课程管理_修改课程")]
        [AdminLog("课程管理", "修改课程")]
        public async Task<ActionResult> Edit(long id, string name, int amount)
        {
            var result = await linkService.EditAsync(id, name, 0, amount);
            if (result <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "修改课程失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "修改课程成功" });
        }
        [HttpPost]
        public async Task<ActionResult> GetLink(long id)
        {
            var model = await linkService.GetModelByIdAsync(id);
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        #endregion

        #region 冻结课程
        //[HttpPost]
        //public async Task<ActionResult> Frozen(long id)
        //{
        //    bool res = await linkService.FrozenAsync(id);
        //    if (!res)
        //    {
        //        return Json(new AjaxResult { Status = 0, Msg = "冻结、解冻课程操作失败" });
        //    }
        //    return Json(new AjaxResult { Status = 1, Msg = "冻结、解冻课程操作成功" });
        //}
        #endregion

        #region 删除课程
        [HttpPost]
        [Permission("课程管理_删除课程")]
        [AdminLog("课程管理", "删除课程")]
        public async Task<ActionResult> Del(long id)
        {
            bool res = await linkService.DelAsync(id);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "删除课程失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "删除课程成功" });
        }
        #endregion
    }
}