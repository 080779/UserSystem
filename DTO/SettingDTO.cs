using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class SettingDTO : BaseDTO
    {
        public string ParamName { get; set; }
        public long LevelId { get; set; }
        public long TypeId { get; set; }
        public string Name { get; set; }
        public string Parm { get; set; }
        public string Description { get; set; }
        public int Sort { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class PHPSettingDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Parm { get; set; }
        public string Description { get; set; }
    }
}
