using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Common
{
    public interface IApiResult
    {

    }

    public class ApiResult : IApiResult
    {
        public string code { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
    public class ApiResultTips : IApiResult
    {
        public string code { get; set; }
        public string message { get; set; }
    }
}
