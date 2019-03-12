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
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

namespace BookMyHotelTests.BookMyHotel_Tenants.UserApp.Tests
{
    [TestClass]
    public class AccountControllerTests
    {
        private readonly AccountController _accountController;

        public AccountControllerTests(IStringLocalizer<AccountController> localizer, IStringLocalizer<BaseController> baseLocalizer, 
            ILogger<AccountController> logger)
        {
            var mockTenantRepo = new Mock<ITenantRepository>();
            var mockConfig = new Mock<IConfiguration>();
            var mockDnsClient = new Mock<ILookupClient>();
            mockTenantRepo.Setup(repo => repo.GetGuest("test@email.com", 123456)).Returns(GetCustomerAsync());
            mockTenantRepo.Setup(repo => repo.AddGuestAsync(GetCustomer(), 123456)).Returns(GetCustomerId());

            var mockCatalogRepo = new Mock<ICatalogRepository>();

            var mockUtilities = new Mock<IUtilities>();

            _accountController = new AccountController(localizer, baseLocalizer, mockTenantRepo.Object, mockCatalogRepo.Object, logger,mockConfig.Object,mockDnsClient.Object);
        }

        [TestMethod]
        public void LoginTest()
        {
            //Act
            var result = _accountController.Login("tenantName", "test@email.com");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
        }

        [TestMethod]
        public void RegisterCustomerTest()
        {
            //Act
            var result = _accountController.Register("tenantName", GetCustomer());

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
        }

        private GuestModel GetCustomer()
        {
            return new GuestModel
            {
                CityCode = "HYD",
                PostalCode = "123",
                Email = "test@gmail.com",
                FirstName = "customer1",
                LastName = "lastName"
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
                LastName = "lastName"
            };
        }
    }
}
