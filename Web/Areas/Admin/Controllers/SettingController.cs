﻿using IMS.Common;
using IMS.IService;
using IMS.Web.App_Start.Attributes;
using IMS.Web.Areas.Admin.Models.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class SettingController : Controller
    {
        public ISettingService settingService { get; set; }
        //[Permission("日志管理_查看日志")]
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        //[Permission("日志管理_查看日志")]
        [AdminLog("系统设置", "查看系统设置")]
        public async Task<ActionResult> List(string keyword,DateTime? startTime,DateTime? endTime,int pageIndex=1)
        {
            SettingListViewModel model = new SettingListViewModel();
            var tilte= await settingService.GetModelByNameAsync("系统标题");
            model.SysTitle = new SettingParm { Id = tilte.Id, Parm = tilte.Param };
            var phone1 = await settingService.GetModelByNameAsync("客服电话");
            model.Phone1 = new SettingParm { Id = phone1.Id, Parm = phone1.Param };
            var phone2 = await settingService.GetModelByNameAsync("客服电话1");
            model.Phone2 = new SettingParm { Id = phone2.Id, Parm = phone2.Param };
            var logo = await settingService.GetModelByNameAsync("系统LOGO");
            model.Logo = new SettingParm { Id = logo.Id, Parm = logo.Param };
            var about = await settingService.GetModelByNameAsync("关于我们");
            model.About = new SettingParm { Id = about.Id, Parm = about.Param };

            return Json(new AjaxResult { Status = 1, Data = model });
        }
        [HttpPost]
        [ValidateInput(false)]
        [AdminLog("系统设置", "编辑系统设置")]
        [Permission("系统设置_系统设置")]
        public async Task<ActionResult> Edit(List<SettingParm> parms)
        {
            if(parms.Count()<=0)
            {
                return Json(new AjaxResult { Status = 0,Msg="无参数"});
            }
            string path = "";
            bool res = false;
            foreach (var item in parms)
            {
                if(item.Parm.Contains(";base64,"))
                {
                    bool flag =ImageHelper.SaveBase64(item.Parm, out path);
                    if(!flag)
                    {
                        return Json(new AjaxResult { Status = 0, Msg = "logo图片保存失败" });
                    }
                    res = await settingService.UpdateAsync(item.Id, path);
                }
                else
                {
                    res = await settingService.UpdateAsync(item.Id, item.Parm);
                }
            }
            if(!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "修改失败" });
            }
            return Json(new AjaxResult { Status = 1,Msg="修改成功" });
        }
    }
}