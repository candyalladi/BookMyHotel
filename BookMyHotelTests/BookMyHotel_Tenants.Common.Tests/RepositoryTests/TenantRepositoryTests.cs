using BookMyHotel.Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Models;
using BookMyHotel_Tenants.UserApp.EF.TenantsDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Assert = Xunit.Assert;

namespace BookMyHotelTests.BookMyHotel_Tenants.Common.Tests.RepositoryTests
{
    [TestClass]
    public class TenantRepositoryTests
    {
        private ITenantRepository _tenantRepository;
        private const int _tenantId = 1;
        private const int _numberOfBookingPurchases = 1;
        private const int _bookingsSold = 1;

        public TenantRepositoryTests()
        {
            _tenantRepository = new MockTenantRepository();
            _tenantRepository.AddGuestAsync(CreateCustomerModel(), _tenantId);
        }
        [Fact]
        public void Setup()
        {
            _tenantRepository = new MockTenantRepository();
            _tenantRepository.AddGuestAsync(CreateCustomerModel(), _tenantId);
        }

        private GuestModel CreateCustomerModel()
        {
            return new GuestModel
            {
                CityCode = "HYD",
                Email = "test@email.com",
                GuestId = 123,
                PostalCode = "12345",
                LastName = "last name",
                FirstName = "first name",
                Password = "pass"
            };
        }

        [Fact]
        public async void GetAllCitiesTest()
        {
            var result = (await _tenantRepository.GetAllCitiesAsync(_tenantId));

            Assert.NotNull(result);
            Assert.Single(result);            
            Assert.Equal("HYD", result[0].CityCode);
            Assert.Equal("Hyderabad", result[0].CityName);
        }

        [Fact]
        public async void GetCityTest()
        {
            var result = await _tenantRepository.GetCityAsync("HYD", _tenantId);

            Assert.NotNull(result);
            Assert.Equal("HYD", result.CityCode);
            Assert.Equal("Hyderabad", result.CityName);
        }

        [Fact]
        public void AddCustomerTest()
        {
            var result = (_tenantRepository.AddGuestAsync(CreateCustomerModel(), _tenantId)).Result;

            Assert.Equal(123, result);
        }

        [Fact]
        public async void GetCustomerTest()
        {
            var result = await _tenantRepository.GetGuest("test@email.com", _tenantId);

            Assert.NotNull(result);
            Assert.Equal("HYD", result.CityCode);
            Assert.Equal("test@email.com", result.Email);
            Assert.Equal(123, result.GuestId);
            Assert.Equal("12345", result.PostalCode);
            Assert.Equal("last name", result.LastName);
            Assert.Equal("first name", result.FirstName);
            Assert.Equal("pass", result.Password);
        }

        [Fact]
        public async void GetRoomPricesTest()
        {
            var result = await _tenantRepository.GetRoomPricesAsync(1, _tenantId);

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal(1, result[0].RoomId);
            Assert.Equal(1, result[0].HotelId);
            Assert.Equal(100, result[0].Price);
            Assert.Equal(1, result[1].RoomId);
            Assert.Equal(2, result[1].HotelId);
            Assert.Equal(80, result[1].Price);
            Assert.Equal(1, result[2].RoomId);
            Assert.Equal(3, result[2].HotelId);
            Assert.Equal(60, result[2].Price);
        }

        [Fact]
        public async void GetRoomsTest()
        {
            List<int> sectionIds = new List<int> { 1, 2 };

            var result = await _tenantRepository.GetRoomsAsync(sectionIds, _tenantId);
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].RoomId);
            Assert.Equal(10, result[0].HotelId);
            Assert.Equal("section 1", result[0].RoomName);
            Assert.Equal(100, result[0].StandardPrice);
            Assert.Equal("Standard", result[0].RoomType);

            Assert.Equal(2, result[1].RoomId);
            Assert.Equal(20, result[1].HotelId);
            Assert.Equal("section 2", result[1].RoomName);
            Assert.Equal(80, result[1].StandardPrice);
            Assert.Equal("Luxury", result[1].RoomType);
        }

        [Fact]
        public async void GetRoomTest()
        {
            var result = await _tenantRepository.GetRoomAsync(1, _tenantId);
            Assert.NotNull(result);
            Assert.Equal(1, result.RoomId);
            Assert.Equal(10, result.HotelId);
            Assert.Equal("section 1", result.RoomName);
            Assert.Equal(100, result.StandardPrice);
            Assert.Equal("Standard", result.RoomType);
        }

        [Fact]
        public void AddBookingPurchaseTest()
        {
            var ticketPurchaseModel = new BookingPurchaseModel
            {
                BookedDate = DateTime.Now,
                BookingPurchaseId = 12,
                GuestId = 6,
                TotalPrice = 5
            };

            var result = (_tenantRepository.AddBookingPurchase(ticketPurchaseModel, _tenantId)).Result;

            Assert.Equal(1, _numberOfBookingPurchases);
            Assert.Equal(12, result);
        }

        [Fact]
        public void AddBookingTest()
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
            List<BookingModel> bookingModels = new List<BookingModel>();
            bookingModels.Add(ticketModel);

            var result =  _tenantRepository.AddBookings(bookingModels, _tenantId).Result;

            Assert.True(result);
        }

        [Fact]
        public void GetBookingsSoldTest()
        {
            var result = (_tenantRepository.GetBookingsSold(1, 1, _tenantId)).Result;

            Assert.Equal(_bookingsSold, result);
        }

        [Fact]
        public async void GetHotelDetailsTest()
        {
            var result = await _tenantRepository.GetHotelDetailsAsync(_tenantId);

            Assert.NotNull(result);
            Assert.Equal("HYD", result.CityCode);
            Assert.Equal("5 Star", result.HotelType);
            Assert.Equal("Pramati", result.HotelName);
            Assert.Equal("123", result.PostalCode);
            Assert.Equal("admin@email.com", result.AdminEmail);
            Assert.Equal("password", result.AdminPassword);
        }

        [Fact]
        public async void GetHotelTypeTest()
        {
            var result = await _tenantRepository.GetHotelTypeAsync("pop", _tenantId);

            Assert.NotNull(result);
            Assert.Equal("pop", result.HotelType);
            Assert.Equal("room short name", result.RoomTypeShortNamePlural);
            Assert.Equal("Luxury", result.RoomTypeName);
            Assert.Equal("5 Star", result.HotelTypeName);
            Assert.Equal("short name", result.RoomTypeShortName);
        }
    }
}
