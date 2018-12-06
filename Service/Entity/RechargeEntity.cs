using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
  public  class RechargeEntity: BaseEntity
    {
        public long UserId { get; set; }
        public virtual UserEntity User { get; set; }
        public int AgentID { get; set; }
        public decimal RechargeableMoney { get; set; }
        public int RechargeStyle { get; set; }
        public int RechargeType { get; set; }
        public int Flag { get; set; }
        public DateTime RechargeDate { get; set; }
        public decimal YuAmount { get; set; }
        public int Recharge001 { get; set; }
        public int Recharge002 { get; set; }
        public decimal Recharge005 { get; set; }
        public decimal Recharge006 { get; set; }

    }
}
