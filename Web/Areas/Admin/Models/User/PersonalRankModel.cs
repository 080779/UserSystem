using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Areas.Admin.Models.User
{
    public class PersonalResultModel
    {
        public PersonalRankModel[] List { set; get; }
        public int PageCount { set; get; }

    }
    public class PersonalRankModel
    {
        public int rankNo { set; get; }
        public string nickName { set; get; }
        public string usercode { set; get; }
        public string uid { set; get; }
        public decimal sroce { set; get; }

    }
}