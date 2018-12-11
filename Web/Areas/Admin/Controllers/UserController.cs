using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Service;
using IMS.Web.App_Start.Filter;
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
    public class UserController : Controller
    {
        #region 属性注入
        private int pageSize = 10;
        public IUserService userService { get; set; }
        public IIdNameService idNameService { get; set; }
        public ISettingService settingService { get; set; }
        public IRegisterService registerService { get; set; }
        //public IOrderService orderService { get; set; }
        #endregion

        #region 会员列表
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        [AdminLog("会员管理", "查看用户管理列表")]
        public async Task<ActionResult> List(int? levelId,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var result = await userService.GetModelListAsync(levelId, keyword, startTime, endTime, pageIndex, pageSize);
            //var set1 = await settingService.GetModelByNameAsync("第三级显示");
            //UserListViewModel model = new UserListViewModel();
            //model.ThreePlay = new SettingModel { Id = set1.Id, Name = set1.Name, Parm = set1.Parm };
            //model.PageCount = result.PageCount; 
            //model.Users = result.Users;
            //model.Levels = await idNameService.GetByTypeNameAsync("会员等级");
            //model.UserUps = (await settingService.GetModelListAsync("会员升级")).Select(s => new SettingModel { Id = s.Id, Parm = s.Parm,Name=s.Name}).ToList();
            //model.Discounts = (await settingService.GetModelListAsync("会员优惠")).Select(s => new SettingModel { Id = s.Id, Parm = s.Parm ,Name=s.Name}).ToList();
            return Json(new AjaxResult { Status = 1, Data = result });
        }
        #endregion

        #region 添加会员
        /*
        [Permission("会员管理_新增会员")]
        [AdminLog("会员管理", "添加用户")]
        public async Task<ActionResult> Add(string mobile,string recommendMobile,string password)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "会员账号不能为空" });
            }
            if (!Regex.IsMatch(mobile, @"^1\d{10}$"))
            {
                return Json(new AjaxResult { Status = 0, Msg = "注册手机号格式不正确" });
            }
            if (string.IsNullOrEmpty(recommendMobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "推荐人账号不能为空" });
            }
            if (string.IsNullOrEmpty(password))
            {
                return Json(new AjaxResult { Status = 0, Msg = "登录密码不能为空" });
            }
            long levelId = await idNameService.GetIdByNameAsync("普通会员");
            long id = await userService.AddAsync(mobile, password, "", levelId, recommendMobile, null, null);
            if (id <= 0)
            {
                if (id == -1)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "会员账号已经存在" });
                }
                if (id == -2)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "推荐人不存在" });
                }
                return Json(new AjaxResult { Status = 0, Msg = "会员添加失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "会员添加成功" });
        }
        */
        #endregion

        #region 升级设置
        /*
        [AdminLog("会员管理", "用户升级管理")]
        [Permission("会员管理_升级设置")] 
        public async Task<ActionResult> UpSet(List<SettingModel> settings)
        {
            if (settings.Count() <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "无参数" });
            }
            foreach (var item in settings)
            {
                if (!Regex.IsMatch(item.Parm, @"^\d*[0-9](|.\d*[0-9]|,\d*[0-9])?$"))
                {
                    return Json(new AjaxResult { Status = 0, Msg = "参数错误" });
                }
            }

            bool flag = await settingService.UpdateAsync(settings.Select(s=>new SettingParm { Id=s.Id,Parm=s.Parm}).ToArray());
            if (!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "修改失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "修改成功" });
        }
        */
        #endregion

        #region 重置密码
        [AdminLog("会员管理", "重置用户密码")]
        [Permission("会员管理_重置密码")]
        public async Task<ActionResult> ResetPwd(long id, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return Json(new AjaxResult { Status = 0, Msg = "登录密码不能为空" });
            }
            long res = await userService.ResetPasswordAsync(id,password);
            if (res <= 0)
            {
                if (id == -1)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "会员不存在" });
                }
                return Json(new AjaxResult { Status = 0, Msg = "重置密码失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "重置密码成功" });
        }
        #endregion

        #region 冻结用户
        [AdminLog("会员管理", "冻结用户")]
        [Permission("会员管理_冻结用户")]
        public async Task<ActionResult> Frozen(long id)
        {
            bool res= await userService.FrozenAsync(id);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "冻结、解冻用户失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "冻结、解冻用户成功" });
        }
        #endregion

        #region 删除用户
        [AdminLog("会员管理", "删除用户")]
        [Permission("会员管理_删除用户")]
        public async Task<ActionResult> Delete(long id)
        {
            bool res = await userService.DeleteAsync(id);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "删除用户失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "删除用户成功" });
        }
        #endregion

        #region 赠送积分
        [AdminLog("会员管理", "赠送积分")]
        [Permission("会员管理_赠送积分")]
        public async Task<ActionResult> Giving(long id,int integral)
        {
            bool res = await userService.AddAntegralAsync(id,integral);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "赠送积分失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "赠送积分成功" });
        }
        #endregion

        #region 导入用户
        //[AdminLog("会员管理", "删除用户")]
        //[Permission("会员管理_删除用户")]
        public async Task<ActionResult> Import(HttpPostedFileBase excelFile)
        {
            var res = ExcelHelper.SaveExecl(excelFile);
            if(!res.Key)
            {
                return Json(new AjaxResult { Status = 0, Msg = res.Value });
            }
            var dt = ExcelHelper.GetDataTable(res.Value);
            long result;
            foreach (DataRow row in dt.Rows)
            {
                result = await userService.AddByExcelAsync(row["电话"].ToString(), row["姓名"].ToString(), 1,"123456",null,row["推荐人"].ToString(),null,null);
                if(result==-2)
                {
                    continue;
                }
                if (result <= 0)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "导入失败" });
                }
            }
            
            return Json(new AjaxResult { Status = 1, Msg = "导入成功" });
        }
        #endregion

        #region 业绩
        public ActionResult PersonalRank()
        {
            return View();
        }

        public ActionResult TeamRank()
        {
            return View();
        }

        public ActionResult SchoolRank()
        {
            return View();
        }

        /// <summary>
        /// 个人业绩
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PersonalRankList(int pageIndex)
        {
            int pageCount = 0;

            using (MyDbContext dbc = new MyDbContext())
            {
                SqlParameter[] parameters = {
                    new SqlParameter("@pageIndex", SqlDbType.Int,4),
                    new SqlParameter("@pageSize", SqlDbType.Int,4),
                    new SqlParameter("@pageCount", SqlDbType.Int,4),
                    new SqlParameter("@totalCount", SqlDbType.Int,4)
                };
                parameters[0].Value = pageIndex;
                parameters[1].Value = pageSize;
                parameters[2].Direction = ParameterDirection.Output;
                parameters[3].Direction = ParameterDirection.Output;

                var list = dbc.Database.SqlQuery<PersonalRankModel>("exec proc_PersonalRankList @pageIndex = @pageIndex,@pageSize =@pageSize,@pageCount = @pageCount output, @totalCount = @totalCount output", parameters).ToList();

                pageIndex -= 1;

                int i = pageIndex * pageSize;
                list.ForEach(s => s.rankNo = ++i);

                int.TryParse(parameters[2].Value.ToString(), out pageCount);
                PersonalResultModel teamRank = new PersonalResultModel();
                teamRank.List = list.ToArray();
                teamRank.PageCount = pageCount;

                return Json(new AjaxResult { Status = 1, Data = teamRank });
            }

        }

        /// <summary>
        /// 团队业绩
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TeamRankList(int pageIndex)
        {
            int pageCount = 0;

            using (MyDbContext dbc = new MyDbContext())
            {
                SqlParameter[] parameters = {
                    new SqlParameter("@pageIndex", SqlDbType.Int,4),
                    new SqlParameter("@pageSize", SqlDbType.Int,4),
                    new SqlParameter("@pageCount", SqlDbType.Int,4),
                    new SqlParameter("@totalCount", SqlDbType.Int,4)
                };
                parameters[0].Value = pageIndex;
                parameters[1].Value = pageSize;
                parameters[2].Direction = ParameterDirection.Output;
                parameters[3].Direction = ParameterDirection.Output;

                var list = dbc.Database.SqlQuery<TeamRankModel>("exec proc_TeamRankList @pageIndex = @pageIndex,@pageSize =@pageSize,@pageCount = @pageCount output, @totalCount = @totalCount output", parameters).ToList();

                pageIndex -= 1;

                int i = pageIndex * pageSize;
                list.ForEach(s => s.rankNo = ++i);

                int.TryParse(parameters[2].Value.ToString(),out pageCount);
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     TeamRankResultModel teamRank = new TeamRankResultModel();
                teamRank.List = list.ToArray();
                teamRank.PageCount = pageCount;

                return Json(new AjaxResult { Status = 1, Data = teamRank });
            }

        }

        /// <summary>
        /// 学校业绩
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SchoolRankList(int pageIndex)
        {
            int pageCount = 0;

            using (MyDbContext dbc = new MyDbContext())
            {
                SqlParameter[] parameters = {
                    new SqlParameter("@pageIndex", SqlDbType.Int,4),
                    new SqlParameter("@pageSize", SqlDbType.Int,4),
                    new SqlParameter("@pageCount", SqlDbType.Int,4),
                    new SqlParameter("@totalCount", SqlDbType.Int,4)
                };
                parameters[0].Value = pageIndex;
                parameters[1].Value = pageSize;
                parameters[2].Direction = ParameterDirection.Output;
                parameters[3].Direction = ParameterDirection.Output;

                var list = dbc.Database.SqlQuery<SchoolRankModel>("exec proc_SchoolRankList @pageIndex = @pageIndex,@pageSize =@pageSize,@pageCount = @pageCount output, @totalCount = @totalCount output", parameters).ToList();

                pageIndex -= 1;

                int i = pageIndex * pageSize;
                list.ForEach(s => s.rankNo = ++i);

                int.TryParse(parameters[2].Value.ToString(), out pageCount);
                SchoolResultModel teamRank = new SchoolResultModel();
                teamRank.List = list.ToArray();
                teamRank.PageCount = pageCount;

                return Json(new AjaxResult { Status = 1, Data = teamRank });
            }

        }
        #endregion
    }
}