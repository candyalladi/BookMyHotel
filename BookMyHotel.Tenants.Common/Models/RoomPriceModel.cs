namespace BookMyHotel_Tenants.Common.Models
{
    public class RoomPriceModel
    {
        public int HotelId { get; set; }
        public int RoomId { get; set; }
        public decimal Price { get; set; }
    }
}
