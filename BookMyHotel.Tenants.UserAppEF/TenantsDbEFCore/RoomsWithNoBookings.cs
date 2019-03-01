using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BookMyHotel_Tenants.UserApp.EF.TenantsDbEFCore
{
    public class RoomsWithNoBookings
    {
        [Key]
        [Column(Order = 0)]
        public int RoomId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string RoomName { get; set; }

        [Column(Order = 2)]
        [StringLength(50)]
        public string RoomType { get; set; }

        [Column(Order = 3)]
        [StringLength(50)]
        public int HotelId { get; set; }

        [Column(Order = 4)]        
        public decimal StandardPrice { get; set; }
    }
}
