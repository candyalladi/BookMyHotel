using System;

namespace BookMyHotel_Tenants.Common.Models
{
    public class BookingPurchaseModel
    {
        public int BookingPurchaseId { get; set; }
        public DateTime BookedDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int GuestId { get; set; }  
        public int OfferId { get; set; }        
    }
}
