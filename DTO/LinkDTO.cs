using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class LinkDTO : BaseDTO
    {
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public string Url { get; set; }
        public string Tip { get; set; }
        public long TypeId { get; set; }
        public string TypeName { get; set; }
        public decimal Link001 { get; set; }
        public decimal link002 { get; set; }
        public int Sort { get; set; }
        public bool IsEnabled { get; set; }
    }
}
