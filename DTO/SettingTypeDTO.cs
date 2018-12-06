using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class SettingTypeDTO : BaseDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Sort { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class SettingSetDTO
    {
        public string TypeName { get; set; }
        public SettingDTO[] Settings { get; set; }
    }
}
