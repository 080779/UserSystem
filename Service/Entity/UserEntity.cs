using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 用户实体类
    /// </summary>
    public class UserEntity : BaseEntity
    {
        public long ShopUID { get; set; } = 0;
        public string UserCode { get; set; }
        public string Mobile { get; set; }
        public string Code { get; set; }
        public string NickName { get; set; }
        public string HeadPic { get; set; }
        public string TrueName { get; set; }
        public decimal Amount { get; set; } = 0;//账户金额
        public decimal FrozenAmount { get; set; } = 0;
        public decimal BonusAmount { get; set; } = 0;//累计佣金
        /// <summary>
        /// 消费金额
        /// </summary>
        public decimal BuyAmount { get; set; } = 0;//消费金额
        public int LevelId { get; set; } = 1;
        public decimal RegMoney { get; set; }
        public string Description { get; set; }
        public string Salt { get; set; } = string.Empty;
        public string Password { get; set; }
        public string TradePassword { get; set; }
        public string ShareCode { get; set; }
        public int ErrorCount { get; set; } = 0;
        public DateTime ErrorTime { get; set; } = DateTime.Now;
        public bool IsEnabled { get; set; } = true;
        public bool IsReturned { get; set; } = false;//是否退过货
        public bool IsUpgraded { get; set; } = false;//是否激活
        public bool IsNull { get; set; } = false;
        public decimal TeamScore { get; set; } = 0;
        public decimal PersonalScore { get; set; } = 0;
        public long RecommendId { get; set; }
        public string RecommendCode { get; set; }
        public string RecommendPath { get; set; }
        public int RecommendGenera { get; set; }
        public int MLevelId { get; set; } = 0;
        public decimal BonusDiffTotal { get; set; } = 0;
        //  public virtual RegisterEntity Regist { get; set; }
    }
}
