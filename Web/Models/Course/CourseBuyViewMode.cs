using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Course
{
    public class CourseBuyViewMode
    {
        public BankAccountDTO BankAccount { get; set; }
        public LinkDTO[] Courses { get; set; }
    }
}