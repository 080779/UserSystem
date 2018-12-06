using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.News
{
    public class NewsListViewModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string CreateTime { get; set; }
    }
}