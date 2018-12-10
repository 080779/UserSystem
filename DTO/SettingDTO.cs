using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class SettingDTO : BaseDTO
    {
        public int? Sort { get; set; }
        public string ParamName { get; set; }
        public int? LevelId { get; set; }
        public string Name { get; set; }
        public string Param { get; set; }
        public string Remark { get; set; }
        public int? ParamTypeId { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class PHPSettingDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Param { get; set; }
        public string Remark { get; set; }
    }
}
