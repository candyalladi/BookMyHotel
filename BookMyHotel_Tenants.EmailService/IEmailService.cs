namespace BookMyHotel_Tenants.EmailService
{
    public interface IEmailService
    {
        void SendEmailToTenants(string fromEmailId, string fromAzureServiceName, string toTenantEmailId, string tenantName);
        void SendEmailToGuests(string fromEmailAddress, string fromHotelName, string toEmailAddress, string guestName, string confirmationMessage);
    }
}
