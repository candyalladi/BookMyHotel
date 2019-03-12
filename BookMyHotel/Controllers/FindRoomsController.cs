using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookMyHotel.Tenants.Common.Interfaces;
using BookMyHotel.ViewModels;
using BookMyHotel_Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Models;
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
        private readonly DnsClient.ILookupClient _client;
        #endregion

        public FindRoomsController(ITenantRepository tenantRepository, ICatalogRepository catalogRepository, IStringLocalizer<FindRoomsController> localizer, IStringLocalizer<BaseController> baseLocalizer,
            ILogger<FindRoomsController> logger, IConfiguration configuration, DnsClient.ILookupClient client)
            : base(baseLocalizer, tenantRepository, configuration, client)
        {
            _tenantRepository = tenantRepository;
            _catalogRepository = catalogRepository;
            _localizer = localizer;
            _logger = logger;
            _client = client;
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

                        var eventDetails = await _tenantRepository.GetRoomAsync(roomId, tenantDetails.TenantId);

                        if (eventDetails != null)
                        {
                            var eventSections = await _tenantRepository.GetRoomPricesAsync(eventId, tenantDetails.TenantId);
                            var seatSectionIds = eventSections.Select(i => i.RoomId).ToList();

                            var seatSections = await _tenantRepository.GetRoomsAsync(seatSectionIds, tenantDetails.TenantId);
                            if (seatSections != null)
                            {
                                var ticketsSold = await _tenantRepository.GetBookingsSold(seatSections[0].RoomId, eventId, tenantDetails.TenantId);

                                FindHotelViewModel viewModel = new FindHotelViewModel
                                {
                                    //BookingDetails = eventDetails,
                                    //Rooms = eventDetails,
                                    //RoomsAvailable = (seatSections[0].SeatRows * seatSections[0].SeatsPerRow) - ticketsSold
                                };

                                return View(viewModel);
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

        [Route("GetAvailableSeats")]
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
                    var ticketsSold = await _tenantRepository.GetBookingsSold(sectionId, eventId, tenantDetails.TenantId);

                    var availableRooms = totalRooms - ticketsSold;
                    return Content(availableRooms.ToString());
                }
                else
                {
                    return View("TenantError", tenant);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "GetAvailableSeats failed for tenant {tenant} and event {eventId}", tenant, eventId);
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

                    var bookingsPurchaseId = await _tenantRepository.AddBookinPurchase(ticketPurchaseModel, tenantDetails.TenantId);

                    List<BookingModel> ticketsModel = BuildTicketModel(bookingId, roomId, roomsCount, bookingsPurchaseId);
                    purchaseResult = await _tenantRepository.AddBookings(ticketsModel, tenantDetails.TenantId);

                    if (purchaseResult)
                        DisplayMessage(_localizer[$"You have successfully booked {roomsCount} rooms(s)."], "Confirmation");
                    else
                        DisplayMessage(_localizer["Failed to book rooms."], "Error");
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