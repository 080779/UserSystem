using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Common.Enums
{
    public enum CashBuyStateEnums
    {
        撤销 = -1,
        进行中 = 0,
        已完成 = 1,
        已关闭 = 2
    }
    public enum CashSellStateEnums
    {
        撤销 = -1,
        进行中 = 0,
        已完成 = 1,
        已关闭 = 2
    }
    public enum CashOrderPayStateEnums
    {
        撤销 = -1,
        交易中 = 0,
        已支付 = 1
    }
    public enum CashOrderConfirmStateEnums
    {
        撤销 = -1,
        交易中 = 0,
        已收款 = 1
    }
    public enum CashOrderStateEnums
    {
        撤销 = -1,
        交易中 = 0,
        已完成 = 1,
        已关闭 = 2
    }
}
