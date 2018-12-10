using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class NewsController : Controller
    {
        public INoticeService noticeService { get; set; }
        [PublicViewBag("新闻详情")]//SYSAuthorizationFilter中含有这个标记的action添加公共的viewbag到布局页中
        public async Task<ActionResult> Info(long id)
        {
            var model = await noticeService.GetModelAsync(id);
            return View(model);
        }

        public async Task<ActionResult> List()
        {
            var res = await noticeService.GetModelListAsync(null, null, null, 1, 20);
            return Json(new AjaxResult { Status=1,Data=res.Notices.Select(n=>new NewsListViewModel{ Code=n.Code,Id=n.Id,CreateTime=n.CreateTime.ToString("[yyyy-MM-dd]")})});
        }
    }
}