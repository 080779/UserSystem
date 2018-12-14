using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Service;
using IMS.Web.App_Start.Attributes;
using IMS.Web.Areas.Admin.Models.User;
using SDMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class ActivateController : Controller
    {
        #region 属性注入
        private int pageSize = 10;
        public IUserService userService { get; set; }
        #endregion

        #region 激活会员列表
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        [AdminLog("会员管理", "查看待激活用户管理列表")]
        public async Task<ActionResult> List(int? levelId,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var result = await userService.GetActivateListAsync(pageIndex, pageSize); 
            return Json(new AjaxResult { Status = 1, Data = result });
        }
        #endregion

        #region 激活会员
        [AdminLog("会员管理", "激活会员")]
        [Permission("会员管理_激活会员")]
        public async Task<ActionResult> Set(long id)
        {
            bool res = await userService.ActivateAsync(id);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "激活会员失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "激活会员成功" });
        }

        [AdminLog("会员管理", "激活会员")]
        [Permission("会员管理_激活会员")]
        public async Task<ActionResult> SetAll(long[] ids)
        {
            if(ids.Count()<=0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "请选择要激活的会员" });
            }
            bool res = await userService.ActivateAllAsync(ids);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "批量激活会员失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "批量激活会员成功" });
        }
        #endregion

        #region 导出未激活用户
        [Permission("会员管理_导出未激活用户")]
        [AdminLog("会员管理", "导出未激活用户")]
        public async Task<ActionResult> Export()
        {
            var res = await userService.GetAllInactiveAsycn();
            UserExportExcelModel[] result = res.Select(o => new UserExportExcelModel
            {
                Amount = o.Amount,
                CreateTime = o.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Mobile = o.Mobile,
                RecommendCode = o.RecommendCode,
                TrueName = o.TrueName,
                IsActivate = o.IsUpgraded ? "已激活" : "未激活",
                LvevlName = o.LevelName
            }).ToArray();
            return File(ExcelHelper.ExportExcel<UserExportExcelModel>(result, "未激活会员信息"), "application/vnd.ms-excel", "未激活会员信息.xls");
        }
        #endregion
    }
}