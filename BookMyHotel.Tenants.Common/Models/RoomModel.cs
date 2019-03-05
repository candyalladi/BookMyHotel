using System.Collections.Generic;

namespace BookMyHotel_Tenants.Common.Models
{
    public enum RoomType
    {
        Standard,
        Delux,
        Luxury
    }

    public class RoomModel
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public string RoomType { get; set; } // Need to check for conversion from enum type
        public int HotelId { get; set; }
        public decimal StandardPrice { get; set; }
    }

    public class RoomManagementModel
    {
        public int RoomManagementId { get; set; }
        public int RoomId { get; set; }
        // Whether the Room is available or not 
        public bool RoomStatus { get; set; }
        public List<string> Services { get; set; }
    }
}
