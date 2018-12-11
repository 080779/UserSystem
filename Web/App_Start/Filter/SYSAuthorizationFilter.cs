using IMS.Common;
using IMS.Common.Newtonsoft;
using IMS.DTO;
using IMS.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.App_Start.Filter
{
    public class SYSAuthorizationFilter: IAuthorizationFilter
    {
        public IAdminService adminUserService = DependencyResolver.Current.GetService<IAdminService>();
        public IPermissionService permissionService = DependencyResolver.Current.GetService<IPermissionService>();
        public IAdminLogService adminLogService = DependencyResolver.Current.GetService<IAdminLogService>();
        public IUserService userService= DependencyResolver.Current.GetService<IUserService>();
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            string path = filterContext.HttpContext.Request.Path;//获取url
            string redirect = path.Split('/')[1].ToLower();
            if(redirect=="admin")
            {
                #region 后台验证权限
                PermissionAttribute attribute = (PermissionAttribute)filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(PermissionAttribute), false).SingleOrDefault();
                PermissionAttribute[] attributes = (PermissionAttribute[])filterContext.ActionDescriptor.GetCustomAttributes(typeof(PermissionAttribute), false);
                //var attributes = filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(true);
                long? adminUserId = (long?)filterContext.HttpContext.Session["Platform_AdminUserId"];
                if (adminUserId == null)
                {
                    if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
                    {
                        return;
                    }
                    if (filterContext.HttpContext.Request.IsAjaxRequest())//判断是否是ajax请求
                    {
                        filterContext.Result = new JsonNetResult { Data = new AjaxResult { Status = 0, Data = "/admin/login/login" } };
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult("/admin/login/login");
                    }
                    return;
                }
                if (attribute == null && attributes.Length <= 0)
                {
                    object[] attrs = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AdminLogAttribute), false);
                    if (attrs != null && attrs.Length > 0)
                    {
                        string ipAddress = CommonHelper.GetWebClientIp();
                        string logDesc = ((AdminLogAttribute)attrs[0]).AdminLog;
                        string permType = ((AdminLogAttribute)attrs[0]).PermissionType;
                        adminLogService.Add(adminUserId.Value, permType, logDesc, ipAddress, "");
                    }
                    return; //如果没有权限检查的attribute就返回，不进行后面的判断
                }
                else if (attribute != null)
                {
                    object[] attrs = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AdminLogAttribute), false);
                    if (attrs != null && attrs.Length > 0)
                    {
                        string ipAddress = CommonHelper.GetWebClientIp();
                        string logDesc = ((AdminLogAttribute)attrs[0]).AdminLog;
                        string permType = ((AdminLogAttribute)attrs[0]).PermissionType;
                        adminLogService.Add(adminUserId.Value, permType, logDesc, ipAddress, "");
                    }
                    if (!adminUserService.HasPermission(adminUserId.Value, attribute.Permission))
                    {
                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                        {
                            filterContext.Result = new JsonNetResult { Data = new AjaxResult { Status = 0, Msg = "没有" + permissionService.GetNameByDesc(attribute.Permission) + "这个权限" } };
                        }
                        else
                        {
                            //filterContext.Result = new ContentResult() { Content = "没有" + permissionService.GetByName(attr.Permission).Description + "这个权限" };
                            filterContext.Result = new RedirectResult("/admin/home/permission?msg=" + "没有" + permissionService.GetNameByDesc(attribute.Permission) + "这个权限");
                        }
                        return;
                    }
                }
                else if (attributes.Length > 0)
                {
                    object[] attrs = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AdminLogAttribute), false);
                    if (attrs != null && attrs.Length > 0)
                    {
                        string ipAddress = CommonHelper.GetWebClientIp();
                        string logDesc = ((AdminLogAttribute)attrs[0]).AdminLog;
                        string permType = ((AdminLogAttribute)attrs[0]).PermissionType;
                        adminLogService.Add(adminUserId.Value, permType, logDesc, ipAddress, "");
                    }
                    foreach (var attr in attributes)
                    {
                        if (!adminUserService.HasPermission(adminUserId.Value, attr.Permission))
                        {
                            if (filterContext.HttpContext.Request.IsAjaxRequest())
                            {
                                filterContext.Result = new JsonNetResult { Data = new AjaxResult { Status = 1, Msg = "没有" + permissionService.GetNameByDesc(attr.Permission) + "这个权限" } };
                            }
                            else
                            {
                                //filterContext.Result = new ContentResult() { Content = "没有" + permissionService.GetByName(attr.Permission).Description + "这个权限" };
                                filterContext.Result = new RedirectResult("/admin/home/permission?msg=" + "没有" + permissionService.GetNameByDesc(attr.Permission) + "这个权限");
                            }
                            return;
                        }
                    }
                    return;
                }
                #endregion
            }
            else
            {
                #region 前台验证权限
                if (filterContext.HttpContext.Request.Cookies["Platform_UserId"] == null)
                {
                    if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
                    {
                        return;
                    }
                    if (filterContext.HttpContext.Request.IsAjaxRequest())//判断是否是ajax请求
                    {
                        filterContext.Result = new JsonNetResult { Data = new AjaxResult { Status = 302, Data = "/user/login" } };
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult("/user/login");
                    }
                    return;
                }
                else
                {                    
                    PublicViewBagAttribute[] attributes = (PublicViewBagAttribute[])filterContext.ActionDescriptor.GetCustomAttributes(typeof(PublicViewBagAttribute), false);
                    if(attributes.Count()<=0)
                    {
                        return;
                    }
                    var user = userService.GetModel(CookieHelper.GetLoginId());
                    foreach(var attr in attributes)
                    {
                        filterContext.Controller.ViewBag.Title = attr.Title;
                        filterContext.Controller.ViewBag.Id = user.Id;
                        filterContext.Controller.ViewBag.Mobile = user.Mobile;
                        filterContext.Controller.ViewBag.LevelName = user.LevelName;
                        filterContext.Controller.ViewBag.Amount = user.Amount;
                        filterContext.Controller.ViewBag.IsUpgraded = user.IsUpgraded;
                    }
                }
                #endregion
            }
        }
    }
}