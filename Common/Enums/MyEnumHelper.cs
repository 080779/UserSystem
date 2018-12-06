using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Common.Enums
{
    public static class MyEnumHelper
    {
        public static string GetEnumName<T>(this int type)
        {
            return Enum.GetName(typeof(T), type);
        }
    }
}
