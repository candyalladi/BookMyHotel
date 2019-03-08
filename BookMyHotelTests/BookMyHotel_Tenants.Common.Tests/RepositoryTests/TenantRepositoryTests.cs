using BookMyHotel.Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Models;
using BookMyHotel_Tenants.UserApp.EF.TenantsDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookMyHotelTests.BookMyHotel_Tenants.Common.Tests.RepositoryTests
{
    [TestClass]
    public class TenantRepositoryTests
    {
        private ITenantRepository _tenantRepository;
        private const int _tenantId = 1368421345;
        private const int _numberOfBookingPurchases = 1;
        private const int _bookingsSold = 1;

        [Xunit.Fact]
        public void Setup()
        {
            _tenantRepository = new MockTenantRepository();
            _tenantRepository.AddGuestAsync(CreateCustomerModel(), _tenantId);
        }

        private GuestModel CreateCustomerModel()
        {
            return new GuestModel
            {
                CityCode = "USA",
                Email = "test@email.com",
                GuestId = 123,
                PostalCode = "12345",
                LastName = "last name",
                FirstName = "first name",
                Password = "pass"
            };
        }

        [TestMethod]
        public async void GetAllCitiesTest()
        {
            var result = (await _tenantRepository.GetAllCitiesAsync(_tenantId));

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);            
            Assert.AreEqual("HYD", result[0].CityCode);
            Assert.AreEqual("Hyderabad", result[0].CityName);
        }

        [TestMethod]
        public async void GetCityTest()
        {
            var result = await _tenantRepository.GetCityAsync("HYD", _tenantId);

            Assert.IsNotNull(result);
            Assert.AreEqual("USA", result.CityCode);
            Assert.AreEqual("United States", result.CityName);
        }

        [TestMethod]
        public void AddCustomerTest()
        {
            var result = (_tenantRepository.AddGuestAsync(CreateCustomerModel(), _tenantId)).Result;

            Assert.AreEqual(123, result);
        }

        [TestMethod]
        public async void GetCustomerTest()
        {
            var result = await _tenantRepository.GetGuest("test@email.com", _tenantId);

            Assert.IsNotNull(result);
            Assert.AreEqual("USA", result.CityCode);
            Assert.AreEqual("test@email.com", result.Email);
            Assert.AreEqual(123, result.GuestId);
            Assert.AreEqual("12345", result.PostalCode);
            Assert.AreEqual("last name", result.LastName);
            Assert.AreEqual("first name", result.FirstName);
            Assert.AreEqual("pass", result.Password);
        }

        [TestMethod]
        public async void GetRoomPricesTest()
        {
            var result = await _tenantRepository.GetRoomPricesAsync(1, _tenantId);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(1, result[0].RoomId);
            Assert.AreEqual(1, result[0].HotelId);
            Assert.AreEqual(100, result[0].Price);
            Assert.AreEqual(2, result[1].RoomId);
            Assert.AreEqual(1, result[1].HotelId);
            Assert.AreEqual(80, result[1].Price);
            Assert.AreEqual(3, result[2].RoomId);
            Assert.AreEqual(1, result[2].HotelId);
            Assert.AreEqual(60, result[2].Price);
        }

        //[TestMethod]
        //public async void GetBookingsForTenantTest()
        //{
        //    var result = await _tenantRepository.GetBookingsSold(_tenantId);
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(2, result.Count);
        //    Assert.AreEqual(1, result[0].Booking);
        //    Assert.AreEqual("Event 1", result[0].EventName);
        //    Assert.AreEqual("Event 1 Subtitle", result[0].SubTitle);
        //    Assert.AreEqual(2, result[1].EventId);
        //    Assert.AreEqual("Event 2", result[1].EventName);
        //    Assert.AreEqual("Event 2 Subtitle", result[1].SubTitle);
        //}

        //[TestMethod]
        //public async void GetRoomTest()
        //{
        //    var result = await _tenantRepository.GetRoomAsync(1, _tenantId);

        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(1, result.RoomId);
        //    Assert.AreEqual("Event 1", result.RoomName);
        //    Assert.AreEqual("Event 1 Subtitle", result.RoomType);
        //}

        [TestMethod]
        public async void GetRoomsTest()
        {
            List<int> sectionIds = new List<int> { 1, 2 };

            var result = await _tenantRepository.GetRoomsAsync(sectionIds, _tenantId);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].RoomId);
            Assert.AreEqual(10, result[0].HotelId);
            Assert.AreEqual("section 1", result[0].RoomName);
            Assert.AreEqual(100, result[0].StandardPrice);
            Assert.AreEqual("Standard", result[0].RoomType);

            Assert.AreEqual(2, result[1].RoomId);
            Assert.AreEqual(20, result[1].HotelId);
            Assert.AreEqual("section 2", result[1].RoomName);
            Assert.AreEqual(80, result[1].StandardPrice);
            Assert.AreEqual("Luxury", result[1].RoomType);
        }

        [TestMethod]
        public async void GetRoomTest()
        {
            var result = await _tenantRepository.GetRoomAsync(1, _tenantId);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RoomId);
            Assert.AreEqual(10, result.HotelId);
            Assert.AreEqual("section 1", result.RoomName);
            Assert.AreEqual(100, result.StandardPrice);
            Assert.AreEqual("Standard", result.RoomType);
        }

        [TestMethod]
        public void AddBookingPurchaseTest()
        {
            var ticketPurchaseModel = new BookingPurchaseModel
            {
                BookedDate = DateTime.Now,
                BookingPurchaseId = 12,
                GuestId = 6,
                TotalPrice = 5
            };

            var result = (_tenantRepository.AddBookinPurchase(ticketPurchaseModel, _tenantId)).Result;

            Assert.IsNotNull(result);
            Assert.AreEqual(_numberOfBookingPurchases, 1);
            Assert.AreEqual(12, result);
        }

        [TestMethod]
        public async void AddBookingTest()
        {
            var ticketModel = new BookingModel
            {
                BookingId = 2,
                RoomId = 4,
                BookingPurchaseId = 50,
                HotelName = "Novatel",
                RoomName = "A506",
                GuestId = 100,
                Checkin_Date = DateTime.Now,
                Checkout_Date = DateTime.Now.AddDays(3),
                GuestName = "Pramati"

            };
            List<BookingModel> ticketModels = new List<BookingModel>();
            ticketModels.Add(ticketModel);

            var result = await _tenantRepository.AddBookings(ticketModels, _tenantId);

            Assert.IsNotNull(result);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetBookingsSoldTest()
        {
            var result = (_tenantRepository.GetBookingsSold(1, 1, _tenantId)).Result;

            Assert.AreEqual(_bookingsSold, result);
        }

        [TestMethod]
        public async void GetHotelDetailsTest()
        {
            var result = await _tenantRepository.GetHotelDetailsAsync(_tenantId);

            Assert.IsNotNull(result);
            Assert.AreEqual("HYD", result.CityCode);
            Assert.AreEqual("5 Star", result.HotelType);
            Assert.AreEqual("Venue 1", result.HotelName);
            Assert.AreEqual("123", result.PostalCode);
            Assert.AreEqual("admin@email.com", result.AdminEmail);
            Assert.AreEqual("password", result.AdminPassword);
        }

        [TestMethod]
        public async void GetHotelTypeTest()
        {
            var result = await _tenantRepository.GetHotelTypeAsync("pop", _tenantId);

            Assert.IsNotNull(result);
            Assert.AreEqual("pop", result.HotelType);
            Assert.AreEqual("event short name", result.RoomTypeShortNamePlural);
            Assert.AreEqual("classic", result.RoomTypeName);
            Assert.AreEqual("type 1", result.HotelTypeName);
            Assert.AreEqual("short name", result.RoomTypeShortName);
        }
    }
}
