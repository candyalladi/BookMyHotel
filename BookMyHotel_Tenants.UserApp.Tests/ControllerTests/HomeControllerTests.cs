using BookMyHotel.Controllers;
using BookMyHotel.Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

namespace BookMyHotelTests.BookMyHotel_Tenants.UserApp.Test.ControllerTests
{
    [TestClass]
    public class HomeControllerTests
    {
        private readonly HomeController _homeController;

        public HomeControllerTests(ILogger<HomeController> logger, IUtilities utilities)
        {
            // Arrange
            var mockCatalogRepo = new Mock<ICatalogRepository>();
            var mockTenantRepo = new Mock<ITenantRepository>();
            var mockUtilities = new Mock<IUtilities>();

            mockCatalogRepo.Setup(repo => repo.GetAllTenantsAsync()).Returns(GetTenants());
            mockTenantRepo.Setup(repo => repo.GetHotelDetailsAsync(1234646)).Returns(GetHotelDetails());

            _homeController = new HomeController(mockCatalogRepo.Object, mockTenantRepo.Object, logger, mockUtilities.Object);
        }

        [Fact]
        public void Index_GetAllTenantDetails()
        {
            //Act
            var result = _homeController.Index();

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        private async Task<HotelModel> GetHotelDetails()
        {
            return new HotelModel
            {
                AdminEmail = "adminEmail",
                AdminPassword = "Password",
                CityCode = "HYD",
                PostalCode = "123",
                HotelName = "Pramati",
                HotelType = "5Star",
                HotelId = 1,
                NumberOfFloors = 6,
                RoomsPerFloor = 200
            };
        }

        private async Task<List<TenantModel>> GetTenants()
        {
            return new List<TenantModel>
            {
                new TenantModel
                {
                    TenantName = "pramati",
                    HotelName = "Pramati Star",
                    ServicePlan = "Standard",
                    CityName = "Hyderabad",
                    TenantId = 1
                },
                new TenantModel
                {
                    TenantName = "pramati123",
                    HotelName = "Pramati 7 Star",
                    ServicePlan = "Standard",
                    CityName = "Bengaluru",
                    TenantId = 2
                },
                new TenantModel
                {
                    TenantName = "pramati7star",
                    HotelName = "Pramati luxury Star",
                    ServicePlan = "Enterprise",
                    CityName = "Mumbai",
                    TenantId = 3
                }
            };
        }
    }
}
