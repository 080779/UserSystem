using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
  public  class TransferDTO : BaseDTO
    {
        public int UserID { get; set; }
        public int ToUserType { get; set; }
        public int ToUserID { get; set; }
        public int ChangeType { get; set; }
        public int MoneyType { get; set; }
        public decimal Amount { get; set; }
        public DateTime ChangeDate { get; set; }
        public int Change001 { get; set; }
        public int Change002 { get; set; }
        public string Change003 { get; set; }
        public string Change004 { get; set; }
        public decimal Change005 { get; set; }
        public decimal Change006 { get; set; }
     
 
    }
    public class TransferADTO : BaseDTO
    {
      
        public string ChangeTypeName { get; set; }
        public decimal Amount { get; set; }
     

    }
}
