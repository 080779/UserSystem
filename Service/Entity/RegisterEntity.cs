using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    public class RegisterEntity : BaseEntity
    {
        public long UserId { get; set; }
    //    public UserEntity User { get; set; }
        public string Name { get; set; }
        public int Sex { get; set; }
        public string TeamName { get; set; }
        public string SchoolName { get; set; }
        public string Profession { get; set; }
        public string ContactPhone { get; set; }
        public string QQ { get; set; }
        public int Flag { get; set; }
        public DateTime? FlagTime { get; set; }
        public Guid GRid { get; set; }
        public string TeamShareCode { get; set; }
    }
}
