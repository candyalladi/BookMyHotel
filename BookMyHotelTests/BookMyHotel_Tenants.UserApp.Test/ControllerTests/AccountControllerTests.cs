using BookMyHotel.Controllers;
using BookMyHotel.Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Models;
using DnsClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace BookMyHotelTests.BookMyHotel_Tenants.UserApp.Test.ControllerTests
{
    [TestClass]
    public partial class AccountControllerTests
    {
        private readonly AccountController _accountController;
        private readonly XunitLogger<AccountController> logger;

        public AccountControllerTests(ITestOutputHelper output)
        {
            var mockTenantRepo = new Mock<ITenantRepository>();
            var mockConfig = new Mock<IConfiguration>();
            var mockDnsClient = new Mock<ILookupClient>();

            var mockLocalizer = new Mock<IStringLocalizer<BaseController>>();
            IStringLocalizer<BaseController> stringLocalizer = mockLocalizer.Object;

            var mockAccountLocalizer = new Mock<IStringLocalizer<AccountController>>();
            IStringLocalizer<AccountController> accountStringLocalizer = mockAccountLocalizer.Object;

            logger = new XunitLogger<AccountController>(output);

            mockTenantRepo.Setup(repo => repo.GetGuest("test@email.com", 1)).Returns(GetCustomerAsync());
            mockTenantRepo.Setup(repo => repo.AddGuestAsync(GetCustomer(), 1)).Returns(GetCustomerId());

            var mockCatalogRepo = new Mock<ICatalogRepository>();
            mockCatalogRepo.Setup(repo => repo.GetAllTenantsAsync()).Returns(GetAllTenants());
            mockCatalogRepo.Setup(repo => repo.GetTenantAsync("tenantName")).Returns(GetTenants());

            var mockUtilities = new Mock<IUtilities>();
            mockUtilities.Setup(utl => utl.GetTenantStatus(1)).Returns(GetTenantStatus());

            _accountController = new AccountController(accountStringLocalizer, stringLocalizer, mockTenantRepo.Object, mockCatalogRepo.Object,
                logger, mockConfig.Object, mockDnsClient.Object);

            _accountController.ControllerContext = new ControllerContext();
            _accountController.ControllerContext.HttpContext = new DefaultHttpContext();
            MockHttpSession mockHttpSession = new MockHttpSession();
            mockHttpSession["SessionUsers"] = JsonConvert.SerializeObject(GetGuests());
            _accountController.ControllerContext.HttpContext.Session =mockHttpSession;

        }

       

        [Fact]
        public void LoginTest()
        {
            //Act
            var result = _accountController.Login("tenantName", "test@email.com");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result.Result);
            Assert.Null(redirectToActionResult.ControllerName);
        }

        [Fact]
        public void RegisterCustomerTest()
        {
            //Act            
            var result = _accountController.Register("tenantName", GetCustomer());

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result.Result);
            Assert.Null(redirectToActionResult.ControllerName);
        }

        [Fact]
        public void LogoutTest()
        {
            //Act
            var result = _accountController.Logout("tenantName", "test@gmail.com");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Bookings", redirectToActionResult.ControllerName);
        }

        #region MockMethods

        private GuestModel GetCustomer()
        {
            return new GuestModel
            {
                CityCode = "HYD",
                PostalCode = "123",
                Email = "test@gmail.com",
                FirstName = "customer1",
                LastName = "lastName",
                GuestId=1,
                RoomId=506,
                TenantName= "tenantName"
            };
        }

        private async Task<int> GetCustomerId()
        {
            return 1;
        }

        private async Task<GuestModel> GetCustomerAsync()
        {
            return new GuestModel
            {
                CityCode = "HYD",
                PostalCode = "123",
                Email = "test@gmail.com",
                FirstName = "customer1",
                LastName = "lastName",
               GuestId = 1,
               TenantName = "tenantName",
               RoomId = 506
            };
        }

        private List<GuestModel> GetGuests()
        {

            return new List<GuestModel> 
            {
                new GuestModel
                {
                    CityCode = "HYD",
                    PostalCode = "123",
                    Email = "test@gmail.com",
                    FirstName = "customer1",
                    LastName = "lastName",
                    GuestId = 1,
                    TenantName = "tenantName",
                    RoomId = 506
                },
                new GuestModel
                {
                    CityCode = "BLR",
                    PostalCode = "213",
                    Email = "test@gmail.com",
                    FirstName = "customer2",
                    LastName = "lastName",
                    GuestId =2,
                    TenantName = "tenantName2",
                    RoomId = 503
                }

            };
        }

        private async Task<TenantModel> GetTenants()
        {
            return new TenantModel
            {
                TenantName = "tenantName",
                CityName = "HYD",
                HotelName = "Pramati",
                ServicePlan = "Standard",
                TenantId = 1,
                TenantIdInString="1"
            };
        }

        private async Task<List<TenantModel>> GetAllTenants()
        {
            return new List<TenantModel>
           {
               new TenantModel
               {
                TenantName = "tenantName",
                CityName = "HYD",
                HotelName = "Pramati",
                ServicePlan = "Standard",
                TenantId = 1,
                TenantIdInString="1"
               },
               new TenantModel
               {
                   TenantName = "tenantName1",
                CityName = "BLR",
                HotelName = "Pramati123",
                ServicePlan = "Standard",
                TenantId = 2,
                TenantIdInString="2"
               }
           };
        }

        private string GetTenantStatus()
        {
            return "Online";
        }

        #endregion
    }
}
