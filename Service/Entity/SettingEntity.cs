using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    public class SettingEntity:BaseEntity
    {
        public int? Sort { get; set; } = 1;
        public string ParamName { get; set; }
        public int? LevelId { get; set; }
        public string Name { get; set; }
        public string Param { get; set; }
        public string Remark { get; set; }
        public int? ParamTypeId { get; set; } = 1;
        public bool IsEnabled { get; set; } = true;
    }
}
