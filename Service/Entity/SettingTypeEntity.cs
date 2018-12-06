using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    public class SettingTypeEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Sort { get; set; } = 1;
        public bool IsEnabled { get; set; } = true;
    }
}
