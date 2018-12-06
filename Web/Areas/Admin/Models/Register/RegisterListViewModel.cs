using IMS.DTO;
using IMS.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Pagination;
namespace IMS.Web.Areas.Admin.Models.Register
{
    public class RegisterListViewModel
    {
        public RegisterDTO[] register { get; set; }
        public long PageCount { get; set; }
    }
}