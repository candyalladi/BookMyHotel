using BookMyHotel.Controllers;
using BookMyHotel.Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace BookMyHotelTests.BookMyHotel_Tenants.UserApp.Test.ControllerTests
{
    [TestClass]
    public class HomeControllerTests
    {
        private readonly HomeController _homeController;
        private readonly XunitLogger<HomeController> _logger;

        public HomeControllerTests(ITestOutputHelper output)
        {
            // Arrange
            var mockCatalogRepo = new Mock<ICatalogRepository>();
            var mockTenantRepo = new Mock<ITenantRepository>();
            var mockUtilities = new Mock<IUtilities>();

            _logger = new XunitLogger<HomeController>(output);

            mockCatalogRepo.Setup(repo => repo.GetAllTenantsAsync()).Returns(GetTenants());
            mockTenantRepo.Setup(repo => repo.GetHotelDetailsAsync(1)).Returns(GetHotelDetails());
            mockUtilities.Setup(utl => utl.GetTenantStatus(1)).Returns(GetTenantStatus());

            _homeController = new HomeController(mockCatalogRepo.Object, mockTenantRepo.Object, _logger, mockUtilities.Object);
            _homeController.ControllerContext = new ControllerContext();
            _homeController.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        private string GetTenantStatus()
        {
            return "Online";
        }

        [Fact]
        public void Index_GetAllTenantDetails()
        {
            //Act
            var result = _homeController.Index();

            // Assert
            var redirectToActionResult = Assert.IsType<ViewResult>(result.Result);
            Assert.NotNull(redirectToActionResult.Model);
            var tenants = redirectToActionResult.Model as List<TenantModel>;
            Assert.Equal(3, tenants.Count);
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
