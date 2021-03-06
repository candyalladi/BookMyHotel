﻿using System.Collections.Generic;
using System.Text;

namespace BookMyHotel_Tenants.UserApp.EF.TenantsDB
{
    public partial class Cities
    {
        public Cities()
        {
            Guests = new HashSet<Guests>();
            Hotel = new HashSet<Hotels>();
        }

        public string CityCode { get; set; }
        public string CityName { get; set; }

        public virtual ICollection<Guests> Guests { get; set; }
        public virtual ICollection<Hotels> Hotel { get; set; }
    }
}
