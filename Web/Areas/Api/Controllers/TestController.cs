using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IMS.Web.Areas.Api.Controllers
{
    public class TestController : ApiController
    {
        public object get()
        {
            return new { Name = "fsdaf", CreateTime = DateTime.Now };
        }
    }
}
