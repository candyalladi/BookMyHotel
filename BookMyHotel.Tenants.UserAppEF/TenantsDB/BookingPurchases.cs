using System;
using System.Collections.Generic;

namespace BookMyHotel_Tenants.UserApp.EF.TenantsDB
{
    public partial class BookingPurchases
    {
        public BookingPurchases()
        {
            Bookings = new HashSet<Bookings>();
        }

        public int BookingPurchaseId { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal PurchaseTotal { get; set; }
        public int GuestId { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Bookings> Bookings { get; set; }
        public virtual Guests Guest { get; set; }
    }
}
