using System;
using System.Collections.Generic;
using System.Text;

namespace BookMyHotel_Tenants.UserApp.EF.CatalogDB
{
    public partial class Servers
    {
        public string ServerName { get; set; }
        public string Location { get; set; }
        public string State { get; set; }
        public string RecoveryState { get; set; }
        public System.DateTime LastUpdated { get; set; }
    }
}
