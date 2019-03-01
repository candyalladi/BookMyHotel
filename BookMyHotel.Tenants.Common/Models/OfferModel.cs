namespace BookMyHotel_Tenants.Common.Models
{
    public class OfferModel
    {
        public int OfferId { get; set; }
        public int OfferValidTillDate { get; set; }
        public decimal Discount { get; set; }
        public int HotelId { get; set; }
        public bool IsOfferAvailable { get; set; }
        public int OfferDays { get; set; }
    }
}
