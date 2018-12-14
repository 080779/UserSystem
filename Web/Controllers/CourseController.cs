﻿using IMS.Common;
using IMS.Common.Enums;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Attributes;
using IMS.Web.Models.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class CourseController : Controller
    {
        public IBankAccountService bankAccountService { get; set; }
        public ILinkService linkService { get; set; }
        public ICourseOrderService courseOrderService { get; set; }
        public IUserService userService { get; set; }
        public readonly long loginUserId = CookieHelper.GetLoginId();
        [HttpGet]
        [PublicViewBag("培训课程")]//SYSAuthorizationFilter中含有这个标记的action添加公共的viewbag到布局页中
        public async Task<ActionResult> Buy()
        {
            var res = await bankAccountService.GetModelByUserIdAsync(1);
            return View(res);
        }
        [HttpPost]
        public async Task<ActionResult> Buy(long id,string imgFile)
        {
            var val = await ImageHelper.Base64SaveAsync(imgFile);
            if(!val.Key)
            {
                return Json(new AjaxResult { Status = 0, Msg = val.Value });
            }
            long res = await courseOrderService.AddAsync(CookieHelper.GetLoginId(),CookieHelper.GetLoginMobile(),id,val.Value);
            if(res<=0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "申请购买失败" });
            }
            return Json(new AjaxResult { Status=1,Msg= "申请购买成功" });
        }

        [HttpGet]
        [PublicViewBag("培训课程")]//SYSAuthorizationFilter中含有这个标记的action添加公共的viewbag到布局页中
        public ActionResult IntegralBuy()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> IntegralBuy(long id)
        {
            long res = await courseOrderService.AddAsync(loginUserId, CookieHelper.GetLoginMobile(), id);
            if (res == -1)
            {
                return Json(new AjaxResult { Status = 0, Msg = "已经购买过该课程" });
            }
            if(res == -2)
            {
                return Json(new AjaxResult { Status = 0, Msg = "账户碳积分不足" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "购买成功" });
        }

        public async Task<ActionResult> List()
        {
            var courses = await linkService.GetByTypeNameIsEnableAsync("培训课程");
            return Json(new AjaxResult { Status = 1, Data = courses });
        }

        public async Task<ActionResult> OrderList()
        {
            var res = await courseOrderService.GetModelListAsync(loginUserId,(int)CourseOrderStateEnum.已完成,null,null,null,1,20);
            return Json(new AjaxResult { Status = 1, Data = res.CourseOrders });
        }
    }
}