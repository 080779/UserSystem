using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Areas.Admin.Models.User
{
    public class SchoolResultModel
    {
        public SchoolRankModel[] List { set; get; }
        public int PageCount { set; get; }

    }
    public class SchoolRankModel
    {
        public int rankNo { set; get; }
        public string schoolName { set; get; }
        public decimal sroce { set; get; }

    }
}