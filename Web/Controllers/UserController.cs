﻿using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{    
    public class UserController : Controller
    {
        public IUserService userService { get; set; }

        #region 登录
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(string mobile,string password,string code)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "账号不能为空" });
            }
            if (string.IsNullOrEmpty(password))
            {
                return Json(new AjaxResult { Status = 0, Msg = "密码不能为空" });
            }
            if (string.IsNullOrEmpty(code))
            {
                return Json(new AjaxResult { Status = 0, Msg = "验证码不能为空" });
            }
            string tempCode = TempData["ValidCode"].ToString();
            if (code != tempCode)
            {
                return Json(new AjaxResult { Status = 0, Msg = "验证码错误" });
            }
            long res = await userService.CheckLoginAsync(mobile, password);
            string message = "";
            switch(res)
            {
                case -1: message = "用户名或密码错误";break;
                case -2: message = "用户名或密码错误"; break;
                case -3: message = "用户已经被冻结"; break;
                default:break;
            }
            if(res<=0)
            {
                return Json(new AjaxResult { Status = 0, Msg = message });
            }
            CookieHelper.Login(res.ToString(), "Platform_UserId", false);
            HttpCookie UserCookie = new HttpCookie("Platform_UserId");
            UserCookie["Mobile"] = await userService.GetMobileByIdAsync(res);
            UserCookie["Id"] = res.ToString();
            Response.AppendCookie(UserCookie);
            return Json(new AjaxResult { Status = 1, Msg = "登录成功", Data = "/home/index" });
        }

        [AllowAnonymous]
        public ActionResult Code()
        {
            ValidCode validCode = new ValidCode();
            string code = validCode.CreateVerifyCode();
            TempData["ValidCode"] = code;
            return File(validCode.CodeImageGetBuffer(code), "image/jpeg");
        }
        #endregion

        public ActionResult Logout()
        {
            if (Request.Cookies["Platform_UserId"] != null)
            {
                HttpCookie UserCookie = Request.Cookies["Platform_UserId"];
                UserCookie.Expires = DateTime.Now.AddDays(-2);
                Response.Cookies.Set(UserCookie);                
            }
            return Redirect("/user/login");
        }

        #region 注册
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register(string recommend="")
        {
            return View((object)recommend);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(string mobile, string password, string code,string recommend)
        {
            if(string.IsNullOrEmpty(recommend))
            {
                return Json(new AjaxResult { Status = 0, Msg = "推荐人账号不能为空" });
            }
            if (string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "账号不能为空" });
            }
            //if (!Regex.IsMatch(mobile, @"^1\d{10}$"))
            //{
            //    return Json(new AjaxResult { Status = 0, Msg = "用户手机号格式不正确" });
            //}
            if (string.IsNullOrEmpty(password))
            {
                return Json(new AjaxResult { Status = 0, Msg = "密码不能为空" });
            }
            if (string.IsNullOrEmpty(code))
            {
                return Json(new AjaxResult { Status = 0, Msg = "验证码不能为空" });
            }
            string tempCode = TempData["ValidCode"].ToString();
            if (code != tempCode)
            {
                return Json(new AjaxResult { Status = 0, Msg = "验证码错误" });
            }
            long res = await userService.AddAsync(mobile, 1, password, null, recommend, null,null);
            string message = "";
            switch (res)
            {
                case -1: message = "推荐人不存在"; break;
                case -2: message = "手机号已经被注册"; break;
                case -3: message = "注册失败"; break;
                case -4: message = "未激活会员不能作为推荐人";break;
                default: break;
            }
            if (res <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = message });
            }
            return Json(new AjaxResult { Status = 1, Msg = "注册成功", Data = "/user/login" });
        }
        #endregion

        #region 推荐二维码
        [PublicViewBag("推荐二维码")]//SYSAuthorizationFilter中含有这个标记的action添加公共的viewbag到布局页中
        public async Task<ActionResult> QrCode()
        {
            string mobile = await userService.GetMobileByIdAsync(CookieHelper.GetLoginId());
            string url = "http://192.168.1.110:8081/user/register?recommend=" + mobile;
            return View((object)url);
        }
        #endregion
    }
}