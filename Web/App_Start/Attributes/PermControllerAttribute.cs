using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.App_Start.Attributes
{
    public class PermControllerAttribute : Attribute
    {
        public string Name { get; set; }
        public PermControllerAttribute(string name)
        {
            this.Name = name;
        }
    }
}