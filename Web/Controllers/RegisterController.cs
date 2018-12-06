using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.Registers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace IMS.Web.Controllers
{    
    public class RegisterController : ApiController
    {
        public IRegisterService registerService { get; set; }
        public IMemberService memberService { get; set; }

        [HttpPost]
        public async Task<ApiResult> Add(RegisterApiModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                return new ApiResult { status = 0, msg = "请输入名称" };
            if (string.IsNullOrWhiteSpace(model.SchoolName))
                return new ApiResult { status = 0, msg = "请输入学校名" };
            if (string.IsNullOrWhiteSpace(model.ContactPhone))
                return new ApiResult { status = 0, msg = "请输入联系方式" };
            if (model.Sex != 0 && !(model.Sex >= 1 & model.Sex <= 2))
                return new ApiResult { status = 0, msg = "性别错误" };
            //if (string.IsNullOrWhiteSpace(model.VerifyCode))
            //    return new ApiResult { status = 0, msg = "请输入验证码" };

            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);

            //if(HttpContext.Current.Session["VerifyCode" + user.Id] == null)
            //    return new ApiResult { status = 0, msg = "验证码已过期" };

            //string code = HttpContext.Current.Session["VerifyCode" + user.Id].ToString();
            //if(!model.VerifyCode.Equals(code))
            //    return new ApiResult { status = 0, msg = "验证码错误" };

            #region 验证数据
            var regModel = await registerService.GetModelByUserIdAsync(user.Id);

            if (regModel != null)
            {
                if (regModel.Flag == 0)
                    return new ApiResult { status = 0, msg = "您已报名，请等待审核" };
                if (regModel.Flag == 1)
                    return new ApiResult { status = 0, msg = "您已成功报名，不能重复操作" };
            }
            bool bTeamName = await registerService.ExistsByTeamNameAsync(model.TeamName);
            if (bTeamName)
                return new ApiResult { status = 0, msg = "该团队名称已被注册" };
            bool bPhone = await registerService.ExistsByContactPhoneAsync(model.ContactPhone);
            if (bTeamName)
                return new ApiResult { status = 0, msg = "该手机号已被注册" };

            if (!string.IsNullOrWhiteSpace(model.AMobile))
            {
                if (model.AMobile.Equals(model.ContactPhone))
                    return new ApiResult { status = 0, msg = "手机号不能重复" };
                var memModel = await memberService.GetModelByMobileAsync(model.AMobile);
                if (memModel != null)
                    return new ApiResult { status = 0, msg = "A成员手机号已被注册" };
            }
            if (!string.IsNullOrWhiteSpace(model.BMobile))
            {
                if (model.BMobile.Equals(model.AMobile) || model.BMobile.Equals(model.ContactPhone))
                    return new ApiResult { status = 0, msg = "手机号不能重复" };
                var memModel = await memberService.GetModelByMobileAsync(model.BMobile);
                if (memModel != null)
                    return new ApiResult { status = 0, msg = "B成员手机号已被注册" };

            }
            if (!string.IsNullOrWhiteSpace(model.CMobile))
            {
                if (model.CMobile.Equals(model.BMobile) || model.CMobile.Equals(model.AMobile) || model.CMobile.Equals(model.ContactPhone))
                    return new ApiResult { status = 0, msg = "手机号不能重复" };
                var memModel = await memberService.GetModelByMobileAsync(model.CMobile);
                if (memModel != null)
                    return new ApiResult { status = 0, msg = "C成员手机号已被注册" };

            }

            if (!string.IsNullOrWhiteSpace(model.DMobile))
            {
                if (model.DMobile.Equals(model.CMobile) || model.DMobile.Equals(model.BMobile) || model.DMobile.Equals(model.AMobile) || model.DMobile.Equals(model.ContactPhone))
                    return new ApiResult { status = 0, msg = "手机号不能重复" };
                var memModel = await memberService.GetModelByMobileAsync(model.DMobile);
                if (memModel != null)
                    return new ApiResult { status = 0, msg = "D成员手机号已被注册" };

            } 
            #endregion

            RegisterDTO dto = new RegisterDTO();
            dto.UserId =  user.Id;
            dto.Name = model.Name;
            dto.Sex = model.Sex;
            dto.TeamName = model.TeamName;
            dto.SchoolName = model.SchoolName;
            dto.Profession = model.Profession;
            dto.ContactPhone = model.ContactPhone;
            dto.QQ = model.QQ;
            dto.Flag = 1;
            dto.FlagTime = DateTime.Now;

            long regid = await registerService.AddAsync(dto);

            if (!string.IsNullOrWhiteSpace(model.AMobile))
                await memberService.AddAsync(regid, model.AName, model.AMobile, 1);
            if (!string.IsNullOrWhiteSpace(model.BMobile))
                await memberService.AddAsync(regid, model.BName, model.BMobile, 1);
            if (!string.IsNullOrWhiteSpace(model.CMobile))
                await memberService.AddAsync(regid, model.CName, model.CMobile, 1);
            if (!string.IsNullOrWhiteSpace(model.DMobile))
                await memberService.AddAsync(regid, model.DName, model.DMobile, 1);
            var res = await registerService.Audit(regid);//审核通过，并匹配已绑定手机的用户，进入团队
            if (!res)
                return new ApiResult { status = 0, msg = "报名失败" };
            else
                return new ApiResult { status = 1, msg = "报名成功" };
        }
        [HttpPost]
        public async Task<ApiResult> AddTest(RegisterApiModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                return new ApiResult { status = 0, msg = "请输入名称" };
            if (string.IsNullOrWhiteSpace(model.SchoolName))
                return new ApiResult { status = 0, msg = "请输入学校名" };
            if (string.IsNullOrWhiteSpace(model.ContactPhone))
                return new ApiResult { status = 0, msg = "请输入联系方式" };
            if (model.Sex != 0 && !(model.Sex >= 1 & model.Sex <= 2))
                return new ApiResult { status = 0, msg = "性别错误" };
            //if (string.IsNullOrWhiteSpace(model.VerifyCode))
            //    return new ApiResult { status = 0, msg = "请输入验证码" };

            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);

            //if(HttpContext.Current.Session["VerifyCode" + user.Id] == null)
            //    return new ApiResult { status = 0, msg = "验证码已过期" };

            //string code = HttpContext.Current.Session["VerifyCode" + user.Id].ToString();
            //if(!model.VerifyCode.Equals(code))
            //    return new ApiResult { status = 0, msg = "验证码错误" };

            var regModel = await registerService.GetModelByUserIdAsync(user.Id);

            //if (regModel != null)
            //{
            //    if (regModel.Flag == 0)
            //        return new ApiResult { status = 0, msg = "您已报名，请等待审核" };
            //    if (regModel.Flag == 1)
            //        return new ApiResult { status = 0, msg = "您已成功报名，不能重复操作" };
            //}
            RegisterDTO dto = new RegisterDTO();
            dto.UserId = user.Id;
            dto.Name = model.Name;
            dto.Sex = model.Sex;
            dto.TeamName = model.TeamName;
            dto.SchoolName = model.SchoolName;
            dto.Profession = model.Profession;
            dto.ContactPhone = model.ContactPhone;
            dto.QQ = model.QQ;
            dto.Flag = 1;
            dto.FlagTime = DateTime.Now;

            long regid = await registerService.AddAsync(dto);
            var res = await registerService.Audit(regid);
            if (!res)
                return new ApiResult { status = 1, msg = "报名失败" };
            else
                return new ApiResult { status = 1, msg = "报名成功" };
        }
        [HttpPost]
        public async Task<ApiResult> RankListWeek()
        {
            var res = await registerService.GetRankListWeekAsync();
            var result = new List<Models.Registers.RankListWeekModel>();

            int n = 1;
            result = res.RankList.Select(o => new Models.Registers.RankListWeekModel
            {
                RankNo = n++,
                SchoolName = o.SchoolName,
                TeamName = o.TeamName
            }).Take(20).ToList();
            return new ApiResult { status = 1, data = result };
        }

        [HttpPost]
        public async Task<ApiResult> RankListTeam()
        {
            var res = await registerService.GetRankListTeamAsync();
            var result = new List<Models.Registers.RankListWeekModel>();

            int n = 1;
            result = res.RankList.Select(o => new Models.Registers.RankListWeekModel
            {
                RankNo = n++,
                SchoolName = o.SchoolName,
                TeamName = o.TeamName
            }).Take(20).ToList();
            return new ApiResult { status = 1, data = result };
        }

        [HttpPost]
        public async Task<ApiResult> RankListPersonal()
        {
            var res = await registerService.GetRankListPersonalAsync();
            var result = new List<Models.Registers.RankListPersonalModel>();

            int n = 1;
            result = res.RankList.Select(o => new Models.Registers.RankListPersonalModel
            {
                RankNo = n++,
                Name = o.Name,
                SchoolName = o.SchoolName,
                TeamName = o.TeamName
            }).Take(20).ToList();
            return new ApiResult { status = 1, data = result };
        }
    }    
}