using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Pagination;

namespace IMS.Web.Areas.Admin.Models.Team
{
    public class TeamUserListViewModel
    {
        public UserDTO[] Users { get; set; }
        public IdNameDTO[] Levels { get; set; }
        public long PageCount { get; set; }
        public SettingDTO[] TeamLevels { get; set; }
    }

    public class UserTeamListResult
    {
        public UserTeamModel[] List { get; set; }
        public long PageCount { get; set; }
        public long TotalCount { get; set; }
    }
    public class UserTeamModel
    {
        public int rankNo { set; get; }
        public long TeamId { get; set; }
        public DateTime? FlagTime { get; set; }
        public string UID { get; set; }
        public string NickName { get; set; }
        public string Name { get; set; }
        public string SchoolName { get; set; }
        public string TeamName { get; set; }
        public string Profession { get; set; }
        public string ContactPhone { get; set; }
        
        public string QQ { get; set; }
        public int TeamCount { get; set; }
    }
}