using System;

namespace BookMyHotel_Tenants.UserApp.EF.TenantsDB
{
    public partial class Bookings
    {
        public int BookingId { get; set; }
        public DateTime Checkin_Date { get; set; }
        public DateTime Checkout_Date { get; set; }
        public string RoomName { get; set; }
        public int RoomId { get; set; }
        public int GuestId { get; set; }
        public string GuestName { get; set; }
        public bool Payment { get; set; }
        public string HotelName { get; set; }
        public int BookingPurchaseId { get; set; }

        public virtual BookingPurchases BookingPurchase { get; set; }
        public virtual RoomPrices Rooms { get; set; }        
    }
}
