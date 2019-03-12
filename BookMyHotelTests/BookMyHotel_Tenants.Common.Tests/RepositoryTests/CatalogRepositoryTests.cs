using BookMyHotel_Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Models;
using BookMyHotel_Tenants.UserApp.EF.CatalogDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

namespace BookMyHotelTests.BookMyHotel_Tenants.Common.Tests.RepositoryTests
{
    [TestClass]
    public class CatalogRepositoryTests
    {
        private ICatalogRepository _catalogRepository;

        public CatalogRepositoryTests()
        {
            //var mockCatalogRepo = new Mock<ICatalogRepository>();
            //mockCatalogRepo.Setup(repo => repo.GetAllTenantsAsync()).Returns(GetAllTenants());
            //mockCatalogRepo.Setup(repo => repo.GetTenantAsync("tenantName")).Returns(GetTenantModel());

            _catalogRepository = new MockCatalogRepository();
            _catalogRepository.Add(SetTenant());
        }
        
        [Fact]
        public void AddTenantTest()
        {
            var result = _catalogRepository.Add(SetTenant());
            Assert.True(result);
        }

        [Fact]
        public void GetTenant()
        {
            var result = _catalogRepository.GetTenantAsync("tenantName");
            Assert.NotNull(result);
        }

        [Fact]
        public void GetTenants()
        {
            var result = _catalogRepository.GetAllTenantsAsync();
            Assert.NotNull(result);
        }

        private Tenants SetTenant()
        {
            return new Tenants
            {
                TenantId = BitConverter.GetBytes(65456464),
                TenantName = "test tenant",
                ServicePlan = "Standard"
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

        private async Task<TenantModel> GetTenantModel()
        {
            return new TenantModel
            {
                TenantName = "tenantName",
                CityName = "HYD",
                HotelName = "Pramati",
                ServicePlan = "Standard",
                TenantId = 1,
                TenantIdInString = "1"
            };
        }
    }
}
