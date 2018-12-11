using IMS.Common;
using IMS.DTO;
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
    public class NewsController : Controller
    {
        private int pageSize = 10;
        public INoticeService noticeService { get; set; }
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        [AdminLog("新闻管理", "查看新闻管理列表")]
        public async Task<ActionResult> List(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var result = await noticeService.GetModelListAsync(keyword, startTime, endTime, pageIndex, pageSize);
            return Json(new AjaxResult { Status = 1, Data = result });
        }
        [ValidateInput(false)]
        [AdminLog("新闻管理", "添加新闻管理")]
        [Permission("新闻管理_新增新闻")]
        public async Task<ActionResult> Add(string code, string content, DateTime failureTime)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Json(new AjaxResult { Status = 0, Msg = "新闻标题不能为空" });
            }
            if (string.IsNullOrEmpty(content))
            {
                return Json(new AjaxResult { Status = 0, Msg = "新闻内容不能为空" });
            }            
            long id = await noticeService.AddAsync(code, content, failureTime,Convert.ToInt64(Session["Platform_AdminUserId"]));
            if (id <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "添加新闻失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "添加新闻成功" });
        }

        public async Task<ActionResult> GetModel(long id)
        {
            var model = await noticeService.GetModelAsync(id);
            return Json(new AjaxResult { Status = 1, Data = model });
        }

        [ValidateInput(false)]
        [AdminLog("新闻管理", "修改新闻管理")]
        [Permission("新闻管理_修改新闻")]
        public async Task<ActionResult> Edit(long id, string code, string content, DateTime failureTime)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Json(new AjaxResult { Status = 0, Msg = "新闻标题不能为空" });
            }
            if (string.IsNullOrEmpty(content))
            {
                return Json(new AjaxResult { Status = 0, Msg = "新闻内容不能为空" });
            }
            bool flag = await noticeService.UpdateAsync(id, code, content, failureTime);

            if (!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "修改新闻失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "修改新闻成功" });
        }
        [AdminLog("新闻管理", "删除新闻管理")]
        [Permission("新闻管理_删除新闻")]
        public async Task<ActionResult> Del(long id)
        {
            bool flag = await noticeService.DeleteAsync(id);
            if (!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "删除新闻失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "删除新闻成功" });
        }
    }
}