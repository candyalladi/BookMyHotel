using System.Collections.Generic;

namespace BookMyHotel_Tenants.UserApp.EF.TenantsDB
{
    public partial class Rooms
    {
        public Rooms()
        {
            RoomPrices = new HashSet<RoomPrices>();
        }

        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public RoomType RoomType { get; set; }
        public int HotelId { get; set; }
        public decimal StandardPrice { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<RoomPrices> RoomPrices { get; set; }
    }
}
