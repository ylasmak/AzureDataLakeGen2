using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poc_PIM_ADLS
{
    public class Settings
    {
        public string TenantID { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string AccountName { get; set; }
    }
}
