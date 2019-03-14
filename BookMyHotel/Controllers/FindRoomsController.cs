using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookMyHotel.Tenants.Common.Interfaces;
using BookMyHotel.ViewModels;
using BookMyHotel_Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Models;
using BookMyHotel_Tenants.EmailService;
using DnsClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace BookMyHotel.Controllers
{
    [Route("{tenant}/FindRooms")]
    public class FindRoomsController : BaseController
    {
        #region Private varibles

        private readonly ITenantRepository _tenantRepository;
        private readonly ICatalogRepository _catalogRepository;
        private readonly IStringLocalizer<FindRoomsController> _localizer;
        private readonly ILogger _logger;
        private readonly ILookupClient _client;
        private readonly IEmailService _emailService;
        #endregion

        public FindRoomsController(ITenantRepository tenantRepository, ICatalogRepository catalogRepository, IStringLocalizer<FindRoomsController> localizer, IStringLocalizer<BaseController> baseLocalizer,
            ILogger<FindRoomsController> logger, IConfiguration configuration, ILookupClient client, IEmailService emailService)
            : base(baseLocalizer, tenantRepository, configuration, client)
        {
            _tenantRepository = tenantRepository;
            _catalogRepository = catalogRepository;
            _localizer = localizer;
            _logger = logger;
            _client = client;
            _emailService = emailService;
        }

        [Route("{FindRooms}")]
        public async Task<IActionResult> FindRooms(string tenant, int roomId, int eventId)
        {
            try
            {
                if (eventId != 0)
                {
                    var tenantDetails = (_catalogRepository.GetTenantAsync(tenant)).Result;
                    if (tenantDetails != null)
                    {
                        SetTenantConfig(tenantDetails.TenantId, tenantDetails.TenantIdInString);

                        var hotelDetails = await _tenantRepository.GetHotelDetailsAsync(tenantDetails.TenantId);

                        if (hotelDetails != null)
                        {
                            var roomPrices = await _tenantRepository.GetRoomPricesAsync(eventId, tenantDetails.TenantId);
                            var roomPricesIds = roomPrices.Select(i => i.RoomId).ToList();

                            var rooms = await _tenantRepository.GetRoomsAsync(roomPricesIds, tenantDetails.TenantId);
                            if (rooms != null)
                            {
                                var bookingsSold = await _tenantRepository.GetBookingsSold(rooms[0].RoomId, eventId, tenantDetails.TenantId);
                                List<FindHotelViewModel> hotelsList = new List<FindHotelViewModel>
                                {
                                    new FindHotelViewModel
                                    {
                                        HotelDetails = hotelDetails,
                                        Rooms = rooms,

                                        RoomsAvailable = (hotelDetails.NumberOfFloors * hotelDetails.RoomsPerFloor) - bookingsSold
                                    }
                                };
                                return View(hotelsList);
                            }
                        }
                    }
                    else
                    {
                        return View("TenantError", tenant);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "FindRooms failed for tenant {tenant} and booking {eventId}", tenant, eventId);
                return View("TenantError", tenant);
            }
            return RedirectToAction("Index", "Bookings", new { tenant });
        }

        [Route("GetAvailableRooms")]
        public async Task<IActionResult> GetAvailableRooms(string tenant, int sectionId, int eventId)
        {
            try
            {
                var tenantDetails = (_catalogRepository.GetTenantAsync(tenant)).Result;
                if (tenantDetails != null)
                {
                    SetTenantConfig(tenantDetails.TenantId, tenantDetails.TenantIdInString);

                    var hotelDetails = await _tenantRepository.GetHotelDetailsAsync(tenantDetails.TenantId);
                    var sectionDetails = await _tenantRepository.GetRoomAsync(sectionId, tenantDetails.TenantId);
                    var totalRooms = hotelDetails.NumberOfFloors * hotelDetails.RoomsPerFloor;
                    var bookingsSold = await _tenantRepository.GetBookingsSold(sectionId, eventId, tenantDetails.TenantId);

                    var availableRooms = totalRooms - bookingsSold;
                    return Content(availableRooms.ToString());
                }
                else
                {
                    return View("TenantError", tenant);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "GetAvailableRooms failed for tenant {tenant} and event {eventId}", tenant, eventId);
                return Content("0");
            }
        }

        [HttpPost]
        [Route("Bookings")]
        public async Task<ActionResult> Bookings(string tenant, int bookingId, int guestId, decimal roomPrice, int roomsCount, int roomId)
        {
            try
            {
                bool purchaseResult = false;

                var ticketPurchaseModel = new BookingPurchaseModel
                {
                    GuestId = guestId,
                    TotalPrice = roomPrice
                };

                var tenantDetails = (_catalogRepository.GetTenantAsync(tenant)).Result;                
                if (tenantDetails != null)
                {
                    SetTenantConfig(tenantDetails.TenantId, tenantDetails.TenantIdInString);

                    var hotelDetails = await _tenantRepository.GetHotelDetailsAsync(tenantDetails.TenantId);
                    var bookingsPurchaseId = await _tenantRepository.AddBookingPurchase(ticketPurchaseModel, tenantDetails.TenantId);

                    List<BookingModel> ticketsModel = BuildTicketModel(bookingId, roomId, roomsCount, bookingsPurchaseId);
                    purchaseResult = await _tenantRepository.AddBookings(ticketsModel, tenantDetails.TenantId);

                    var guest = HttpContext.Session.GetObjectFromJson<List<GuestModel>>("SessionUsers");

                    if (purchaseResult)
                    {
                        DisplayMessage(_localizer[$"You have successfully booked {roomsCount} rooms(s)."], "Confirmation");

                        var fromHotelEmailId = hotelDetails.AdminEmail;
                        var fromHotelName = hotelDetails.HotelName;
                        var toEmailAddress = guest[0].Email;
                        var fullName = $"{guest[0].FirstName} {guest[0].LastName}";
                        var confirmMessage = _localizer[$"You have successfully booked {roomsCount} rooms(s)."];

                        _emailService.SendEmailToGuests(fromHotelEmailId, fromHotelName, toEmailAddress, fullName, confirmMessage);
                        
                    }
                    else
                    {
                        DisplayMessage(_localizer["Failed to book rooms."], "Error");
                    }
                }
                else
                {
                    return View("TenantError", tenant);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Booking rooms failed for tenant {tenant} and room {roomId}", tenant, bookingId);
                return View("TenantError", tenant);
            }
            return RedirectToAction("Index", "Bookings", new { tenant });

        }
    }
}