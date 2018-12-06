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
    public class LinkController : Controller
    {
        public ILinkService linkService { get; set; } 

         #region 图片列表
        [HttpGet]
        public ActionResult List(long typeId=1)
        {
            return View(typeId);
        }
        [HttpPost]
        public async Task<ActionResult> List(long typeId, bool flag = true)
        {
            var res = await linkService.GetByTypeIdAsync(typeId);
            return Json(new AjaxResult { Status = 1, Data = res });
        }
        #endregion

        #region 添加图片
        [HttpPost]
        public async Task<ActionResult> Add(long typeId, string name, string imgUrl, string url,int sort)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new AjaxResult { Status = 0, Msg = "图片名不能为空" });
            }
            var res = await ImageHelper.Base64SaveAsync(imgUrl);
            if(!res.Key)
            {
                return Json(new AjaxResult { Status = 0, Msg = res.Value });
            }
            long id = await linkService.AddAsync(typeId, name, res.Value, url, sort);
            if (id <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "添加图片失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "添加图片成功", Data = "/admin/link/list" });
        }
        #endregion

        #region 修改图片
        [HttpPost]
        public async Task<ActionResult> Edit(long id, string name,string imgUrl,string url, int sort)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new AjaxResult { Status = 0, Msg = "图片名不能为空" });
            }

            var res = await ImageHelper.Base64SaveAsync(imgUrl);
            if (!res.Key)
            {
                return Json(new AjaxResult { Status = 0, Msg = res.Value });
            }

            var result = await linkService.EditAsync(id, name, res.Value, url, sort);
            if (result <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "修改图片失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "修改图片成功" });
        }
        [HttpPost]
        public async Task<ActionResult> GetLink(long id)
        {
            var model = await linkService.GetModelByIdAsync(id);
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        #endregion

        #region 冻结图片
        [HttpPost]
        public async Task<ActionResult> Frozen(long id)
        {
            bool res = await linkService.FrozenAsync(id);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "冻结、解冻图片操作失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "冻结、解冻图片操作成功" });
        }
        #endregion

        #region 删除图片
        [HttpPost]
        public async Task<ActionResult> Del(long id)
        {
            bool res = await linkService.DelAsync(id);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "删除图片失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "删除图片成功" });
        }
        #endregion
    }
}