using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service
{
    public static class ServiceHelper
    {
        public async static Task<string> GetStringParamAsync(this MyDbContext dbc, string name)
        {
            return await dbc.GetParameterAsync<SettingEntity>(s => s.Name == name, s => s.Param);
        }

        public async static Task<decimal> GetDecimalParamAsync(this MyDbContext dbc, string name)
        {
            decimal param;
            decimal.TryParse(await dbc.GetParameterAsync<SettingEntity>(s => s.Name == name, s => s.Param), out param);
            return param;
        }
    }
}
