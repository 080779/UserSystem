using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.App_Start.Attributes
{
    public class PermActionAttribute : Attribute
    {
        public string Name { get; set; }
        public PermActionAttribute(string name=null)
        {
            this.Name = name;
        }
    }
}