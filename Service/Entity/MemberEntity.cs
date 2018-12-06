using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 成员实体类
    /// </summary>
    public class MemberEntity : BaseEntity
    {
        public long TeamId { get; set; }
        public string Name { get; set; }//
        public string Mobile { get; set; }//
        public int AddType { get; set; }//
        public long BindUserId { get; set; }
        public DateTime ? BindTime { get; set; }
        public Guid GRid { get; set; }
    }
}
