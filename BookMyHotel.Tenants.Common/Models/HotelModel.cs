using System.ComponentModel.DataAnnotations;

namespace BookMyHotel_Tenants.Common.Models
{
    public partial class HotelModel
    {
        public int HotelId { get; set; }

        public string HotelName { get; set; }

        public string AdminEmail { get; set; }

        [DataType(DataType.Password)]
        public string AdminPassword { get; set; }

        public string PostalCode { get; set; }

        public string CityCode { get; set; }

        public string HotelType { get; set; }

        public string DatabaseName { get; set; }

        public string DatabaseServerName { get; set; }

    }
}
