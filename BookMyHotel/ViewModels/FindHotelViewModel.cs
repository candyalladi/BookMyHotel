using BookMyHotel_Tenants.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookMyHotel.ViewModels
{
    public class FindHotelViewModel
    {
        public BookingModel BookingDetails { get; set; }
        public int RoomId { get; set; }
        public List<RoomModel> Rooms { get; set; }
        public int RoomsAvailable { get; set; }
    }
}
