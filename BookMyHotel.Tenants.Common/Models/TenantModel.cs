namespace BookMyHotel_Tenants.Common.Models
{
    public partial class TenantModel
    {
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string ServicePlan { get; set; }
        public string HotelName { get; set; }
        public string CityName { get; set; }
        public string TenantIdInString { get; set; }
        public string RecoveryState { get; set; }
        public System.DateTime LastUpdated { get; set; }
    }
}
