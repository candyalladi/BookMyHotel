using BookMyHotel.Controllers;
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
    public class BookingsControllerTests
    {
        private readonly BookingsController _bookingsController;

        public BookingsControllerTests(IStringLocalizer<BaseController> baseLocalizer, ILogger<BookingsController> logger)
        {
            var mockCatalogRepo = new Mock<ICatalogRepository>();
            mockCatalogRepo.Setup(repo => repo.GetTenantAsync("testTenant")).Returns(GetTenantModel());

            var mockUtilities = new Mock<IUtilities>();
            var mockTenantRepo = new Mock<ITenantRepository>();
            var mockDnsClient = new Mock<ILookupClient>();
            var mockConfig = new Mock<IConfiguration>();
            var mockBaseLocalizer = new Mock<IStringLocalizer<BaseController>>();
            var mocklogger = new Mock<ILogger<BookingsController>>();

            mockTenantRepo.Setup(repo => repo.GetHotelDetailsAsync(12345)).Returns(GetHotel());
            mockTenantRepo.Setup(repo => repo.GetHotelTypeAsync("Classic", 12345)).Returns(GetHotelType());
            mockTenantRepo.Setup(repo => repo.GetAllCitiesAsync(12345)).Returns(GetCities());
            mockTenantRepo.Setup(repo => repo.GetRoomsAsync(new List<int> { 506, 503 }, 1)).Returns(GetRooms());
            mockTenantRepo.Setup(repo => repo.GetRoomAsync(506, 1)).Returns(GetRoom);
            mockTenantRepo.Setup(repo => repo.GetCityAsync("HYD", 1)).Returns(GetCity);
            mockTenantRepo.Setup(repo => repo.GetGuest("test@email.com", 1)).Returns(GetGuest);
            mockTenantRepo.Setup(repo => repo.GetOffer(1, 1)).Returns(GetOffer);
            mockTenantRepo.Setup(repo => repo.GetOffersForTenant(1)).Returns(GetOffersForTenant);
            mockTenantRepo.Setup(repo => repo.GetRoomPricesAsync(506, 1)).Returns(GetRoomPrices);
            mockTenantRepo.Setup(repo => repo.GetBookingsSold(506, 1, 1)).Returns(GetBookingsSolds);

            _bookingsController = new BookingsController(mockTenantRepo.Object, mockCatalogRepo.Object, mockBaseLocalizer.Object, mocklogger.Object, mockConfig.Object, mockDnsClient.Object, mockUtilities.Object);

        }

        private async Task<int> GetBookingsSolds()
        {
            return 1;
        }

        private async Task<List<RoomPriceModel>> GetRoomPrices()
        {
            return new List<RoomPriceModel>
            {
                new RoomPriceModel
                {
                    HotelId = 1,
                    Price = 10000,
                    RoomId = 506
                },
                new RoomPriceModel
                {
                    HotelId = 2,
                    Price = 10000,
                    RoomId = 503
                }
            };
        }

        private async Task<List<OfferModel>> GetOffersForTenant()
        {
            return new List<OfferModel>
            {
                new OfferModel
                {
                    Discount = 30,
                    HotelId = 1,
                    IsOfferAvailable = true,
                    OfferDays = 3,
                    OfferId = 1,
                    OfferValidTillDate = DateTime.Now.AddDays(2)
                },
                new OfferModel
                {
                    Discount = 20,
                    HotelId = 2,
                    IsOfferAvailable = true,
                    OfferDays = 2,
                    OfferId = 2,
                    OfferValidTillDate = DateTime.Now.AddDays(2)
                },
            };
        }

        private async Task<OfferModel> GetOffer()
        {
            return new OfferModel
            {
                Discount = 30,
                HotelId = 1,
                IsOfferAvailable = true,
                OfferDays = 3,
                OfferId = 1,
                OfferValidTillDate = DateTime.Now.AddDays(2)
            };
        }

        private async Task<GuestModel> GetGuest()
        {
            return new GuestModel
            {
                CityCode = "HYD",
                Email = "test@email.com",
                FirstName = "Kal",
                GuestId = 506,
                LastName = "Dep",
                PostalCode = "500004",
                RoomId = 506,
                TenantName = "Pramati"
            };
        }

        private async Task<CityModel> GetCity()
        {
            return new CityModel
            {
                CityCode = "HYD",
                CityName = "Hyderabad"
            };
        }

        private async Task<RoomModel> GetRoom()
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

        [Fact]
        public void Index_ReturnsView()
        {
            // Act
            var result = _bookingsController.Index("testTenant");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<BookingModel>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());

        }

        private async Task<TenantModel> GetTenantModel()
        {
            return new TenantModel
            {
                HotelName = "Venue 1",
                ServicePlan = "Standard",
                TenantId = 12345,
                TenantIdInString = "12345",
                TenantName = "testTenant"
            };
        }

        private async Task<HotelModel> GetHotel()
        {
            return new HotelModel
            {
                HotelName = "Pramati 1",
                PostalCode = "741",
                CityCode = "HYD",
                HotelType = "Classic"
            };
        }

        private async Task<HotelTypeModel> GetHotelType()
        {
            return new HotelTypeModel
            {
                HotelType = "5Star",
                HotelTypeName = "Luxury Suites",
                RoomTypeShortName = "KingsSuite",
                RoomTypeShortNamePlural = "Kings Suite",
                RoomTypeName = "Kings Suite"
            };
        }

        private async Task<List<CityModel>> GetCities()
        {
            return new List<CityModel>
            {
                new CityModel
                {
                    CityCode = "HYD",
                    CityName = "Hyderabad"
                },
                new CityModel
                {
                    CityCode = "MUM",
                    CityName = "Mumbai"
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

        private async Task<List<BookingModel>> GetBookings()
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
