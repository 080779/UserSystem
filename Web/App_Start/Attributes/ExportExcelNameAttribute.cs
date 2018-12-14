using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.App_Start.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ExportExcelNameAttribute : Attribute
    {
        public string Name { get; set; }
        public ExportExcelNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}