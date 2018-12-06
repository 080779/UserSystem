using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.TakeCash;
using IMS.Web.Models.User;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;

namespace IMS.Web.Controllers
{    
    public class TakeCashController : ApiController
    {
        private static ILog log = LogManager.GetLogger(typeof(OrderController));
        public ITakeCashService takeCashService { get; set; }
        public IIdNameService idNameService { get; set; }
        public IUserService userService { get; set; }
        public async Task<ApiResult> List(TakeCashListModel model)
        {
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            TakeCashSearchResult res = await takeCashService.GetModelListAsync(user.Id, null, null, null, null, model.PageIndex, model.PageSize);
            TakeCashListApiModel result = new TakeCashListApiModel();
            result.takeCashes = res.TakeCashes.Select(t=>new TakeCash { createTime=t.CreateTime,amount=t.Amount,description=t.Description,payTypeName=t.PayTypeName,stateName=t.StateName});
            result.pageCount = res.PageCount;
            return new ApiResult { status = 1, data = result };
        }
        public async Task<ApiResult> Apply(TakeCashApplyModel model)
        {
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            if (model.Amount<=0)
            {
                return new ApiResult { status = 0, msg = "提现金额必须大于零" };
            }
          //  if (user.Id != 157)
            //{
            //    if (model.Amount % 100 != 0)
            //    {
            //        return new ApiResult { status = 0, msg = "提现金额必是100的倍数" };
            //    }
            //}
            //if (model.PayTypeId <= 0)
            //{
            //    return new ApiResult { status = 0, msg = "提现收款类型id必须大于零" };
            //}
            model.PayTypeId = 40;
            var userres= await userService.GetModelAsync(user.Id);

            if(user==null)
            {
                return new ApiResult { status = 0, msg = "用户不存在"};
            }

            if (userres.Amount < model.Amount)
                return new ApiResult { status = 0, msg = "账户余额不足" };

            log.DebugFormat($"提现 takeCashService TakeToWxMone code :{userres.Code} ,#1");

            //调用微信接口，企业付款到零钱
            var wxResult = await TakeToWxMoney(model.Amount, userres.Code);

            log.DebugFormat($"提现 takeCashService TakeToWxMone code :{userres.Code} ,#2");
            if (!wxResult.result_code.Contains("SUCCESS"))
                return new ApiResult { status = 0, msg = wxResult.return_msg };

            long id = await takeCashService.AddAsync(user.Id, model.PayTypeId, model.Amount, "佣金提现", wxResult.partner_trade_no, wxResult.payment_no,DateTime.Parse(wxResult.payment_time));
            if(id<=0)
            {
                if(id==-1)
                {
                    return new ApiResult { status = 0, msg = "用户不存在" };
                }
                if (id == -2)
                {
                    return new ApiResult { status = 0, msg = "用户账户余额不足" };
                }
                if(id==-4)
                {
                    return new ApiResult { status = 0, msg = "-4" };
                }
                log.DebugFormat($"提现 takeCashService.AddAsync：申请提现失败，id:{id}");
                return new ApiResult { status = 0, msg="提现失败" };
            }
            log.DebugFormat($"提现 takeCashService TakeToWxMone :申请提现成功 {userres.Code}，id:{id}");

            return new ApiResult { status = 1, msg = "提现成功" };
        }
        public async Task<ApiResult> PayTypes()
        {
            var result = await idNameService.GetByTypeNameAsync("收款方式");
            var res = result.Select(i => new { id = i.Id, name = i.Name });
            return new ApiResult { status = 1, data = res };
        }
        //提现到微信零钱
        //接口文档：https://pay.weixin.qq.com/wiki/doc/api/tools/mch_pay.php?chapter=14_2
        private async Task<WxTransferResult> TakeToWxMoney(decimal Amount,string openid)
        {
            WxTransferResult result = new WxTransferResult();
            try
            {
                WeChatTransfers weChatTrans = new WeChatTransfers();
                weChatTrans.amount = Math.Truncate(Amount * 100).ToString();
                weChatTrans.openid = openid;
                weChatTrans.desc = "提现到零钱";
                weChatTrans.spbill_create_ip = WebHelper.GetClientIp();

                string parm = HttpClientHelper.BuildParam(weChatTrans);
                string key = System.Configuration.ConfigurationManager.AppSettings["KEY"];
                parm = parm + "&key=" + key;
                string sign = CommonHelper.GetMD5(parm);
                HttpClient httpClient = new HttpClient();
                string xml = HttpClientHelper.SerializeXml(weChatTrans, sign);
                log.DebugFormat($"TakeToWxMoney response xml：{xml}");

                //  string res = await HttpClientHelper.GetResponseByPostXMLAsync(httpClient, xml, "https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers");
                string res = await HttpClientHelper.PostHttpsAsync(httpClient, xml, "https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers", "cyds.wohuicn.com");


                log.DebugFormat($"TakeToWxMoney result xml：{res}");
                //                string res = @"<xml><return_code><![CDATA[SUCCESS]]></return_code>
                //<return_msg><![CDATA[]]></return_msg>
                //<mch_appid><![CDATA[wxbe49b62a3cb300da]]></mch_appid>
                //<mchid><![CDATA[1312108801]]></mchid>
                //<nonce_str><![CDATA[gpgms78aay]]></nonce_str>
                //<result_code><![CDATA[SUCCESS]]></result_code>
                //<partner_trade_no><![CDATA[gpgms78aayr5]]></partner_trade_no>
                //<payment_no><![CDATA[1000018301201810253156081639]]></payment_no>
                //<payment_time><![CDATA[2018-10-25 17:29:34]]></payment_time>
                //</xml>";
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(res);

                result.return_code = xmlDoc.SelectSingleNode("xml/return_code").InnerText;
                result.return_msg = xmlDoc.SelectSingleNode("xml/return_msg").InnerText;
                if (result.return_code == "SUCCESS")
                {
                    result.result_code = xmlDoc.SelectSingleNode("xml/result_code").InnerText;
                    result.partner_trade_no = xmlDoc.SelectSingleNode("xml/partner_trade_no").InnerText;
                    result.payment_no = xmlDoc.SelectSingleNode("xml/payment_no").InnerText;
                    result.payment_time = xmlDoc.SelectSingleNode("xml/payment_time").InnerText;
                }
            }
            catch (Exception ex)
            {
                log.DebugFormat($"TakeToWxMoney Exception：{ex}");

                result.return_code = "ERROR";
                result.return_msg = "提现错误";
            }
            return result;
        }


    }    

    public class WxTransferResult
    {
        public string return_code { set; get; }
        public string return_msg { set; get; }
        public string result_code { set; get; }
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string partner_trade_no { set; get; }
        /// <summary>
        /// 企业付款成功，返回的微信付款单号
        /// </summary>
        public string payment_no { set; get; }
        /// <summary>
        /// 企业付款成功时间
        /// </summary>
        public string payment_time { set; get; }
    }
}