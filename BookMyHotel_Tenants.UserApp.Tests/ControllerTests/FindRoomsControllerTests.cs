﻿using BookMyHotel.Controllers;
using BookMyHotel.Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Models;
using DnsClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

namespace BookMyHotelTests.BookMyHotel_Tenants.UserApp.Test.ControllerTests
{
    [TestClass]
    public class FindRoomsControllerTests
    {
        private readonly FindRoomsController _findRoomsController;

        public FindRoomsControllerTests(IStringLocalizer<FindRoomsController> localizer, IStringLocalizer<BaseController> baseLocalizer, 
            ILogger<FindRoomsController> logger)
        {
            var mockTenantRepo = new Mock<ITenantRepository>();
            var mockCatalogRepo = new Mock<ICatalogRepository>();
            var mockConfig = new Mock<IConfiguration>();
            var mockDnsClient = new Mock<ILookupClient>();

            var roomPrices = GetRoomPrices();
            mockTenantRepo.Setup(repo => repo.GetRoomAsync(1, 12345)).Returns(GetRoomModel());
            mockTenantRepo.Setup(repo => repo.GetRoomPricesAsync(1, 12345)).Returns(roomPrices);            

            var seatSectionIds = roomPrices.Result.ToList().Select(i => i.RoomId).ToList();
            mockTenantRepo.Setup(r => r.GetRoomsAsync(seatSectionIds, 12345)).Returns(GetRooms());
            mockTenantRepo.Setup(r => r.GetRoomAsync(1, 12345)).Returns(GetRoomModel());
            mockTenantRepo.Setup(r => r.GetBookingsSold(1, 1, 12345)).Returns(GetBookingsPurchased());
            mockTenantRepo.Setup(r => r.AddBookings(GetBookingsModel(), 12345)).Returns(GetBooleanValue());
            mockTenantRepo.Setup(r => r.AddBookinPurchase(GetBookingPurchaseModel(), 12345)).Returns(GetBookingId());

            var mockUtilities = new Mock<IUtilities>();

            _findRoomsController = new FindRoomsController(mockTenantRepo.Object, mockCatalogRepo.Object, localizer, baseLocalizer, logger, mockConfig.Object, mockDnsClient.Object);
        }

        [Fact]
        public void FindSeatsTests_BookingId_Null()
        {
            var result = _findRoomsController.FindRooms("tenantName", 0,1);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Bookings", redirectToActionResult.ControllerName);
        }

        [Fact]
        public void FindSeatsTests_BookingId_NotNull()
        {
            var result = _findRoomsController.FindRooms("tenantName", 1,1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<BookingModel>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public void GetAvailableRoomsTest()
        {
            var result = _findRoomsController.GetAvailableRooms("tenantName", 1, 1);

            // Assert
            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Equal("290", contentResult.Content);

        }

        [Fact]
        public void BookingPurchaseTests()
        {
            var result = _findRoomsController.PurchaseTickets("tenantName", 1, 5, 100, 2, 1);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Bookings", redirectToActionResult.ControllerName);
        }

        private async Task<BookingModel> GetBookingModel()
        {
            return new BookingModel
            {
                Checkin_Date = DateTime.Now,
                Checkout_Date = DateTime.Now.AddDays(3),
                BookingId = 1,
                GuestName = "Serenades",
                RoomName = "D506",
                RoomId = 506,
                BookingPurchaseId = 1,
                GuestId = 506,
                HotelName = "Pramati"
            };
        }

        private async Task<int> GetBookingId()
        {
            return 11;
        }

        private async Task<bool> GetBooleanValue()
        {
            return true;
        }

        private async Task<int> GetBookingsPurchased()
        {
            return 10;
        }

        private async Task<List<RoomPriceModel>> GetRoomPrices()
        {
            return new List<RoomPriceModel>
            {
                new RoomPriceModel
                {
                    RoomId = 1,
                    Price = 100,
                    HotelId = 1
                },
                new RoomPriceModel
                {
                    RoomId = 2,
                    Price = 1500,
                    HotelId = 1
                }
            };
        }

        private async Task<List<RoomModel>> GetRooms()
        {
            return new List<RoomModel>
            {
                new RoomModel
                {
                    RoomId = 506,
                    RoomName = "A123",
                    HotelId = 1,
                    StandardPrice = 10000
                },
                new RoomModel
                {
                    RoomId = 503,
                    RoomName = "B123",
                    HotelId = 2,
                    StandardPrice = 10000
                },
            };
        }

        private async Task<RoomModel> GetRoomModel()
        {
            return new RoomModel
            {
                HotelId = 1,
                RoomId = 506,
                RoomName = "A Delux",
                RoomType = "Delux",
                StandardPrice = 4000
            };
        }

        private BookingPurchaseModel GetBookingPurchaseModel()
        {
            return new BookingPurchaseModel
            {
                BookedDate = DateTime.Now,
                BookingPurchaseId = 1,
                GuestId = 506,
                OfferId = 2,
                TotalPrice = 10000
            };
        }

        private List<BookingModel> GetBookingsModel()
        {
            return new List<BookingModel>
            {
                new BookingModel
                {
                    Checkin_Date = DateTime.Now,
                    Checkout_Date = DateTime.Now.AddDays(3),
                    BookingId = 1,
                    GuestName = "Serenades",
                    RoomName = "D506",
                    RoomId = 506,
                    BookingPurchaseId = 1,
                    GuestId = 506,
                    HotelName = "Pramati"
                },
                new BookingModel
                {
                    Checkin_Date = DateTime.Now,
                    Checkout_Date = DateTime.Now.AddDays(3),
                    BookingId = 2,
                    GuestName = "Test",
                    RoomName = "D503",
                    RoomId = 503,
                    BookingPurchaseId = 2,
                    GuestId = 503,
                    HotelName = "Novatel"
                }
            };
        }
    }
}
