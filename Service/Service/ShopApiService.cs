using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using log4net;

namespace IMS.Service.Service
{
    public class ShopApiService : IShopApiService
    {
        private static ILog log = LogManager.GetLogger(typeof(ShopApiService));

        private readonly string KEY = "bcac865b677a6935";
        private readonly string SHOP_API_URL = "http://shunya.wohuicn.com";
        /// <summary>
        /// 获取商城账户余额
        /// </summary>
        /// <returns></returns>
        public async Task<decimal> GetBalanceAsync(long uid)
        {
            decimal balance = 0;
            string apiUrl = SHOP_API_URL + "/api/payment/balance";
            string code = CommonHelper.GetMD5(uid + KEY).ToLower();
          //  string data = string.Format("uid={0}&code={1}",uid,code);
            GetBalanceParam balanceParam = new GetBalanceParam();
            balanceParam.uid = uid;
            balanceParam.code = code;
            HttpClient httpClient = new HttpClient();
            try
            {
                string context = await HttpClientHelper.GetResponseByPostAsync(httpClient, balanceParam, apiUrl);

                var res = JsonConvert.DeserializeObject<AccountResult>(context);
                if (res != null)
                    balance = res.data.balance;
            }
            catch (Exception ex)
            {
                log.Error("GetBalanceAsync :"+ ex.ToString());
            }
            return balance;
             
        }
        /// <summary>
        /// 修改会员账户余额
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="type">0为减少，1为增加</param>
        /// <param name="num"></param>
        /// <returns></returns>
        public async Task<bool> SetBalanceAsync(long uid,int type,decimal num,int from_type)
        {
            int tType = 0;
            if (from_type == 1) //A积分
                tType = 13;
            else
                tType = 14;
            string apiUrl = SHOP_API_URL + "/api/payment/setBalance";
            string code = CommonHelper.GetMD5(uid + KEY).ToLower();
            // string data = string.Format("uid={0}&code={1}&type={2}&num={3}", uid, code,type,num);
            SetBalanceParam param = new SetBalanceParam();
            param.uid = uid;
            param.type = type;
            param.code = code;
            param.num = num;
            param.from_type = tType;//
            HttpClient httpClient = new HttpClient();
            try
            {
                string context = await HttpClientHelper.GetResponseByPostAsync(httpClient, param, apiUrl);

                var res = JsonConvert.DeserializeObject<AccountResult>(context);
                if (res.code.Equals("success"))
                    return true;
            }
            catch (Exception ex)
            {
                log.Error("SetBalanceAsync :" + ex.ToString());
            }
            return false;

        }
        /// <summary>
        /// 验证交易密码
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public async Task<bool> CheckTradePassAsync(long uid, string pass)
        {
            string apiUrl = SHOP_API_URL + "/api/payment/tradePass";
            string code = CommonHelper.GetMD5(uid + KEY).ToLower();
         //   string data = string.Format("uid={0}&code={1}&pass={2}", uid, code, pass);
            CheckTradePassParam param = new CheckTradePassParam();
            param.uid = uid;
            param.code = code;
            param.pass = pass;
            HttpClient httpClient = new HttpClient();
            try
            {
                string context = await HttpClientHelper.GetResponseByPostAsync(httpClient, param, apiUrl);

                var res = JsonConvert.DeserializeObject<AccountResult>(context);
                if (res.code.Equals("success"))
                    return true;
            }
            catch (Exception ex)
            {
                log.Error("SetBalanceAsync :" + ex.ToString());
            }
            return false;

        }
    }
    public class AccountResult
    {
        public string code { get; set; }
        public string message { get; set; }
        public AccountBalance data { get; set; }
    }
    public class AccountBalance
    {
        public decimal balance { get; set; }
    }

    public class GetBalanceParam
    {
        public long uid { get; set; }
        public string code { get; set; }
    }
    public class SetBalanceParam
    {
        public long uid { get; set; }
        public string code { get; set; }
        public int type { get; set; }

        public decimal num { get; set; }
        public int from_type { get; set; }

    }
    public class CheckTradePassParam
    {
        public long uid { get; set; }
        public string code { get; set; }
        public string pass { get; set; }
        
    }
}
