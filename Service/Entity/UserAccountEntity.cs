using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 用户每天奖金记录实体类
    /// </summary>
    public class UserAccountEntity : BaseEntity
    {
        public long UserId { get; set; }
        public virtual UserEntity User { get; set; }
        public long ShopUID { get; set; }
        public decimal Amount { get; set; }// 
        public decimal BonusAmount { get; set; }// 

    }
}
