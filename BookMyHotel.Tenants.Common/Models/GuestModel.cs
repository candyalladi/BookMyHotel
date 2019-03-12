using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookMyHotel_Tenants.Common.Models
{
    public partial class GuestModel
    {
        public int GuestId { get; set; }

        [BindProperty]
        [Required]
        [MinLength(50)]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string PostalCode { get; set; }

        public string CityCode { get; set; }     
        
        public int RoomId { get; set; }

        public string TenantName { get; set; }
    }
}
