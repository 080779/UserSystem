using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.RecommendTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class RecommendTreeController : Controller
    {
        public IUserService userService { get; set; }
        private readonly string KEY = System.Configuration.ConfigurationManager.AppSettings["SHOPKEY"];

        #region 直推图页面
        [HttpGet]
        [PublicViewBag("直推图")]//SYSAuthorizationFilter中含有这个标记的action添加公共的viewbag到布局页中
        public ActionResult Info()
        {
            return View(CookieHelper.GetLoginId());
        }
        #endregion

        #region 直推图获取数据
        public async Task<string> Get(long uid, string id)
        {
            //if (string.IsNullOrEmpty(token))
            //{
            //    return "token不能为空";
            //}

            //if (!Valid(token))
            //{
            //    return "token_invalid";
            //}

            StringBuilder sb = new StringBuilder();
            if (id != "#")
            {
                uid = long.Parse(id);
            }
            UserRecommendTreeDTO user;
            UserRecommendTreeDTO[] list;
            string treeText;
            if (uid != -32423)
            {
                if (uid != 0)
                {
                    user = await userService.GetModelTreeAsync(uid);
                    list = await userService.GetRecommendListAsync(user.Id);
                    treeText = Treetext(user.Mobile, user.Amount, user.LevelName);
                }
                else
                {
                    list = new UserRecommendTreeDTO[0];
                    treeText = "查询无结果";
                }
            }
            else
            {
                //list = await userService.GetRecommendListAsync(uid);
                list = new UserRecommendTreeDTO[0];
                treeText = "查询无结果";
            }
            if (id == "#")
            {
                sb.Append("\"text\":\"" + treeText + "\",\"expanded\":\"false\",\"state\":{\"opened\":\"true\"}");
            }
            if (list.Count() > 0)
            {
                if (id == "#")
                {
                    sb.Append(",\"children\":[{");
                }

                for (int i = 0; i < list.Count(); i++)
                {

                    var list2 = await userService.GetRecommendListAsync(list[i].Id);
                    if (list2.Count() > 0)
                    {
                        sb.Append("\"text\":\"" + Treetext(list[i].Mobile, list[i].Amount, list[i].LevelName) + "\",\"children\":true,\"id\":\"" + list[i].Id + "\"");
                    }
                    else
                    {
                        sb.Append("\"text\":\"" + Treetext(list[i].Mobile, list[i].Amount, list[i].LevelName) + "\"");
                    }

                    if (i != list.Count() - 1)
                    {
                        sb.AppendLine("},{");
                    }
                }
                if (id == "#")
                {
                    sb.Append("}]");
                }
            }
            return "[{" + sb.ToString() + "}]";
        }
        #endregion

        #region 直推图搜索数据
        public async Task<string> Search(string mobile, string token, string id)
        {
            if (string.IsNullOrEmpty(token))
            {
                return "token不能为空";
            }

            if (!Valid(token))
            {
                return "token_invalid";
            }
            long res;
            if (!string.IsNullOrEmpty(mobile))
            {
                res = await userService.GetIdByMobile(mobile);
                if (res <= 0)
                {
                    res = -32423;
                }
            }
            else
            {
                res = 1;
            }            
            return await Get(res, id);
        }
        #endregion

        #region 直推人上级
        public async Task<string> Up(string token, long uid, string id)
        {
            if (string.IsNullOrEmpty(token))
            {
                return "token不能为空";
            }

            if (!Valid(token))
            {
                return "token_invalid";
            }
            long res;
            if (uid != 1)
            {
                res = await userService.GetUserRecommendIdAysnc(uid);
            }
            else
            {
                res = 1;
            }

            return await Get(res, id);
        }
        #endregion

        #region token验证
        private bool Valid(string token)
        {
            string validToken = CommonHelper.GetMD5(DateTime.Now.ToString("yyyyMMdd") + KEY).ToLower();
            if(token!=validToken)
            {
                return false;
            }
            return true;
        }
        #endregion

        private string Treetext(string mobile,decimal amount, string levelName)
        {
            string treeText = "";
            treeText = "<span style='color:green;'>" + mobile + "</span>|<span style='color:green;'>" + levelName + "</span>|<span style='color:green;'>" + amount + "</span> ";
            return treeText;
        }
    }
}