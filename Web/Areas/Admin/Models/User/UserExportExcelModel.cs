using IMS.Web.App_Start.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Areas.Admin.Models.User
{
    public class UserExportExcelModel
    {
        [ExportExcelName("会员账号")]
        public string Mobile { get; set; }
        [ExportExcelName("会员等级")]
        public string LvevlName { get; set; }
        [ExportExcelName("姓名")]
        public string TrueName { get; set; }
        [ExportExcelName("碳积分")]
        public decimal Amount { get; set; }
        [ExportExcelName("推荐人账号")]
        public string RecommendCode { get; set; }
        [ExportExcelName("激活状态")]
        public string IsActivate { get; set; }
        [ExportExcelName("注册时间")]
        public string CreateTime { get; set; }
    }
}