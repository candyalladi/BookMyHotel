using System;
using System.Collections.Generic;
using System.Text;

namespace BookMyHotel_Tenants.UserApp.EF.CatalogDB
{
    public class Tenants
    {
        public byte[] TenantId { get; set; }
        public string TenantName { get; set; }
        public string ServicePlan { get; set; }
        public string RecoveryState { get; set; }
        public System.DateTime LastUpdated { get; set; }
    }
}
