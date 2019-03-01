using System.Collections.Generic;

namespace BookMyHotel_Tenants.UserApp.EF.TenantsDB
{
    public partial class RoomPrices
    {
        public RoomPrices()
        {
            Bookings = new HashSet<Bookings>();
        }

        public int HotelId { get; set; }
        public int RoomId { get; set; }
        public decimal Price { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Bookings> Bookings { get; set; }
        public virtual Bookings Booking { get; set; }
        public virtual Rooms Room { get; set; }
    }
}
