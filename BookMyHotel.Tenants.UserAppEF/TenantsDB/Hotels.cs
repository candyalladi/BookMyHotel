namespace BookMyHotel_Tenants.UserApp.EF.TenantsDB
{
    public partial class Hotels
    {
        public int HotelId { get; set; }
        public string HotelName { get; set; }
        public string HotelType { get; set; }
        public string AdminEmail { get; set; }
        public string AdminPassword { get; set; }
        public string PostalCode { get; set; }
        public string CityCode { get; set; }
        public string Lock { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual Cities CityCodeNavigation { get; set; }
        public virtual HotelTypes HotelTypeNavigation { get; set; }
    }
}
