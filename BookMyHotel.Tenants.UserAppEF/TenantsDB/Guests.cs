using System.Collections.Generic;

namespace BookMyHotel_Tenants.UserApp.EF.TenantsDB
{
    public partial class Guests
    {
        public Guests()
        {
            BookingPurchases = new HashSet<BookingPurchases>();
        }

        public int GuestId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PostalCode { get; set; }
        public string CityCode { get; set; }
        public int RoomId { get; set; }
        public string TenantName { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<BookingPurchases> BookingPurchases { get; set; }
        public virtual Cities CityCodeNavigation { get; set; }
    }
}
