using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class LogDTO : BaseDTO
    {
        public long UserId { get; set; }
        public string LogName { get; set; }//
        public string LogCode { get; set; }//
        public string Description { get; set; }

    }
}
