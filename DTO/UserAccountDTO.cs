using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class UserAccountDTO : BaseDTO
    {
        public long UserId { get; set; }
        public long UID { get; set; }
        public decimal Amount { get; set; }// 
        public decimal BonusAmount { get; set; }// 
    }

    public class CalcUserAccountDTO
    { 
        public long UID { get; set; }
        public decimal BonusAmount { get; set; }
    }
}
