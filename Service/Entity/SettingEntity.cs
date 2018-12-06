using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    public class SettingEntity:BaseEntity
    {
        public string ParamName { get; set; }
        public long? LevelId { get; set; }
        public string Name { get; set; }
        public string Parm { get; set; }
        public string Description { get; set; }
        public long SettingTypeId { get; set; }
        public virtual SettingTypeEntity SettingType { get; set; }
        public int Sort { get; set; } = 1;
        public bool IsEnabled { get; set; } = true;
    }
}
