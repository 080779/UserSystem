using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 图片链接实体类
    /// </summary>
    public class LinkEntity : BaseEntity
    {
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public string Url { get; set; }
        public string Tip { get; set; }
        public long TypeId { get; set; } = 1;
        public string TypeName { get; set; }
        public decimal Link001 { get; set; } = 0;
        public decimal link002 { get; set; } = 0;
        public int Sort { get; set; } = 1;
        public bool IsEnabled { get; set; } = true;
    }
}
