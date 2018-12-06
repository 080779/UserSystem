using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Common.Enums
{
    public enum CourseOrderStateEnum
    {
        已取消 = -1,
        进行中 = 1,
        已完成 = 2
    }
    public enum CourseOrderPayTypeEnum
    {
        银行转账 = 1,
        微信支付 = 2
    }
}
