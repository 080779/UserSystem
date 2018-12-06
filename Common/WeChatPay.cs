using IMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Common
{
    public class WeChatPay
    {
        public string appid { get; set; }= System.Configuration.ConfigurationManager.AppSettings["APPID"];
        public string mch_id { get; set; } = System.Configuration.ConfigurationManager.AppSettings["mch_id"];//"1312108801";
        public string nonce_str { get; set; } = CommonHelper.GetCaptcha(10);
        public string sign_type { get; set; } = "MD5";
        public string body { get; set; } = "test";
        public string detail { get; set; } = "testdetail";
        public string out_trade_no { get; set; } = CommonHelper.GetCaptcha(12);
        public string fee_type { get; set; } = "CNY";
        public string total_fee { get; set; } = "1";
        public string notify_url { get; set; } = System.Configuration.ConfigurationManager.AppSettings["notify_url"];//"http://1823.demo.wohuicn.com/wxpay.ashx";
        public string trade_type { get; set; } = "JSAPI";
        public string openid { get; set; } = "";
    }

    public class WeChatTransfers
    {
        public string mch_appid { get; set; } = System.Configuration.ConfigurationManager.AppSettings["APPID"];
        public string mchid { get; set; } = System.Configuration.ConfigurationManager.AppSettings["mch_id"];//"1312108801";
        /// <summary>
        /// 随机字符串
        /// </summary>
        public string nonce_str { get; set; } = CommonHelper.GetRand(12);
       // public string sign { get; set; } 
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string partner_trade_no { get; set; } = CommonHelper.CreateNo();
        /// <summary>
        /// 用户openid
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 校验用户姓名选项
        /// NO_CHECK：不校验真实姓名
        /// FORCE_CHECK：强校验真实姓名
        /// </summary>
        public string check_name { get; set; } = "NO_CHECK";
        /// <summary>
        /// 收款用户姓名
        /// </summary>
        //public string re_user_name { get; set; }
        /// <summary>
        /// 金额,企业付款金额，单位为分
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 企业付款备注
        /// </summary>
        public string desc { get; set; }
        /// <summary>
        /// Ip地址
        /// </summary>
        public string spbill_create_ip { get; set; } = "";
    }
}