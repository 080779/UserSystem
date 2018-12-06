using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class MemberDTO : BaseDTO
    {
        public long TeamId { get; set; }
        public string Name { get; set; }//
        public string Mobile { get; set; }//
        public int AddType { get; set; }//
        public long BindUserId { get; set; }
        public DateTime? BindTime { get; set; }
        public Guid GRid { get; set; }
    }
}
