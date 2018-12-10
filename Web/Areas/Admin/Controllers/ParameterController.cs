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
    public class ParameterController : Controller
    {
        #region 构造函数注入
        public ISettingTypeService settingTypeService { get; set; }
        public ISettingService settingService { get; set; }
        #endregion

        #region 参数类别列表
        [HttpGet]
        public ActionResult TypeList()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> TypeList(bool flag = true)
        {
            var model = await settingTypeService.GetModelListAsync();
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        #endregion

        #region 添加参数类别
        [HttpPost]
        public async Task<ActionResult> AddType(string name, string description, int sort)
        {
            long res = await settingTypeService.AddAsync(name, description, sort);
            if (res <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "添加权限参数失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "添加权限参数成功", Data = "/admin/idname/typelist" });
        }
        #endregion

        #region 修改参数类别
        [HttpPost]
        public async Task<ActionResult> EditType(long id, string name, string description, int sort)
        {
            long res = await settingTypeService.EditAsync(id, name, description, sort);
            if (res <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "修改权限参数失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "修改权限参数成功", Data = "/admin/idname/typelist" });
        }

        [HttpPost]
        public async Task<ActionResult> GetType(long id)
        {
            var model = await settingTypeService.GetModelAsync(id);
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        #endregion

        #region 冻结、解冻参数类别
        [HttpPost]
        public async Task<ActionResult> FrozenType(long id)
        {
            bool res = await settingTypeService.FrozenAsync(id);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "冻结、解冻权限参数失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "冻结、解冻权限参数成功" });
        }
        #endregion

        #region 删除参数类别
        [HttpPost]
        public async Task<ActionResult> DelType(long id)
        {
            bool res = await settingTypeService.DelAsync(id);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "删除参数类别失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "删除参数类别成功" });
        }
        #endregion

        #region 参数列表
        [HttpGet]
        public ActionResult List(long typeId)
        {
            return View(typeId);
        }
        [HttpPost]
        public async Task<ActionResult> List(int typeId, bool flag = true)
        {
            var model = await settingService.GetByTypeIdAsync(typeId);
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        #endregion

        #region 添加参数
        [HttpPost]
        public async Task<ActionResult> Add(string name, string parm, string description, int typeId, int sort=1)
        {
            var res = await settingService.AddAsync(name, parm, description, sort, typeId);
            if (res <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "添加参数失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "添加参数成功" });
        }
        #endregion

        #region 修改参数
        [HttpPost]
        public async Task<ActionResult> Edit(long id, string name, string parm, string description, int sort)
        {
            var res = await settingService.EditAsync(id, name, parm, description, sort);
            if (res <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "修改参数失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "修改参数成功" });
        }
        [HttpPost]
        public async Task<ActionResult> GetParameter(long id)
        {
            var model = await settingService.GetModelByIdAsync(id);
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        #endregion

        #region 冻结、解冻参数
        [HttpPost]
        public async Task<ActionResult> Frozen(long id)
        {
            bool res = await settingService.FrozenAsync(id);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "冻结、解冻参数失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "冻结、解冻参数成功" });
        }
        #endregion

        #region 删除参数
        [HttpPost]
        public async Task<ActionResult> Del(long id)
        {
            bool res = await settingService.DelAsync(id);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "删除参数失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "删除参数成功" });
        }
        #endregion
    }
}