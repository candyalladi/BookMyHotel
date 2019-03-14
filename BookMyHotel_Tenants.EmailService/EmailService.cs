using System;
using System.Collections.Generic;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BookMyHotel_Tenants.EmailService
{
    public class EmailService : IEmailService
    {
        public EmailService()
        {

        }

        /// <summary>
        /// Sending an email to Hotels who registers as Tenants
        /// </summary>
        /// <param name="fromEmailId">From Sys Admin email</param>
        /// <param name="fromAzureServiceName">Could Platform name</param>
        /// <param name="toTenantEmailId">To HotelId</param>
        /// <param name="tenantName">Hotel Name</param>
        public void SendEmailToTenants(string fromEmailId, string fromAzureServiceName, string toTenantEmailId, string tenantName)
        {
            var msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress(fromEmailId, fromAzureServiceName));

            String registerMessage = "Successfully Registered !!! Thank you for registering your hotel with us. You will receive email about the confirmation.";
            msg.AddTo(toTenantEmailId,tenantName);            
            msg.SetSubject(registerMessage);

            String subject = "Register Confirmation.";
            String body = "Successfully registered.";

            msg.AddContent(MimeType.Text, subject);
            msg.AddContent(MimeType.Html, body);
        }

        /// <summary>
        /// Sending an email to Customers who books the room in Hotels
        /// </summary>
        /// <param name="fromEmailId">Hotel Email Id</param>
        /// <param name="fromHotelName">Hotel name</param>
        /// <param name="toEmailId">Guest/Customer email id</param>
        /// <param name="guestName">Customer/Guest Name</param>
        public void SendEmailToGuests(string fromEmailId, string fromHotelName, 
            string toEmailId, string guestName, string confirmationMessage)
        {
            var msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress(fromEmailId, fromHotelName));
            msg.AddTo(toEmailId);

           //String bookingMessage = "Successfully Booked !!! Thank you for booking at our hotel. You will receive email about the confirmation.";
            msg.SetSubject(confirmationMessage);

            String subject = "Booking Confirmation.";
            String body = "Successfully booked.";

            msg.AddContent(MimeType.Text, subject);
            msg.AddContent(MimeType.Html, body);
        }
    }
}
