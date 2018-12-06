using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Areas.Admin.Models.User
{
    public class TeamRankResultModel
    {
        public TeamRankModel[] List { set; get; }
        public int PageCount { set; get; }

    }
    public class TeamRankModel
    {
        public int rankNo { set; get; }
        public string teamName { set; get; }
        public decimal sroce { set; get; }

    }
}