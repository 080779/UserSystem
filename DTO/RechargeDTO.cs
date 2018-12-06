using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace IMS.DTO
{
  public  class RechargeDTO : BaseDTO
    {

        public long UserID { get; set; }
        public string UserCode { get; set; }
        public decimal RechargeableMoney { get; set; }
        public int RechargeStyle { get; set; }
        public string RechargeStyleName { get; set; }
        public int Flag { get; set; }
        public string  FlagName { get; set; }
        public DateTime RechargeDate { get; set; }
        public decimal YuAmount { get; set; }
        public string  RechargeTypeName { get; set; }
        public int AgentID { get; set; }
        public string Recharge001Name { get; set; }
        public int Recharge002 { get; set; }
        public string Recharge003 { get; set; }
        public string Recharge004 { get; set; }
        public decimal Recharge005 { get; set; }
        public decimal Recharge006 { get; set; }
    

    }
}
