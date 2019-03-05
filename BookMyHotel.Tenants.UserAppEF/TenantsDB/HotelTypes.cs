using System.Collections.Generic;

namespace BookMyHotel_Tenants.UserApp.EF.TenantsDB
{
    public partial class HotelTypes
    {
        public HotelTypes()
        {
            Hotel = new HashSet<Hotels>();
        }

        public string HotelType { get; set; }
        public string HotelTypeName { get; set; }
        public string RoomTypeName { get; set; }
        public string RoomTypeShortName { get; set; }
        public string RoomTypeShortNamePlural { get; set; }

        public virtual ICollection<Hotels> Hotel { get; set; }
    }
}
