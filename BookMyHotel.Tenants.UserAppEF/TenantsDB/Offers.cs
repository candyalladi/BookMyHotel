using System;
using System.Collections.Generic;
using System.Text;

namespace BookMyHotel_Tenants.UserApp.EF.TenantsDB
{
    public class Offers
    {
        public int OfferId { get; set; }
        public DateTime OfferValidTillDate { get; set; }
        public decimal Discount { get; set; }
        public int HotelId { get; set; }
        public bool IsOfferAvailable { get; set; }
        public int OfferDays { get; set; }
        public string Lock { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual Hotels HotelIdNavigation { get; set; }
        public virtual HotelTypes HotelTypeNavigation { get; set; }
    }
}
