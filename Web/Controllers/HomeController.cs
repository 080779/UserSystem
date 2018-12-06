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

namespace IMS.Web.Controllers
{
    public class HomeController : Controller
    {
        [PublicViewBag("首页")]//SYSAuthorizationFilter中含有这个标记的action添加公共的viewbag到布局页中
        public ActionResult Index()
        {
            return View();
        }
    }
}