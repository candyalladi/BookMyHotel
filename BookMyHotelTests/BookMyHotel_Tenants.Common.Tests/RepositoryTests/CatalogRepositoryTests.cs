using BookMyHotel_Tenants.Common.Interfaces;
using BookMyHotel_Tenants.UserApp.EF.CatalogDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookMyHotelTests.BookMyHotel_Tenants.Common.Tests.RepositoryTests
{
    [TestClass]
    public class CatalogRepositoryTests
    {
        private ICatalogRepository _catalogRepository;

        [TestInitialize]
        public void SetUp()
        {
            _catalogRepository = new MockCatalogRepository();
            _catalogRepository.Add(SetTenant());
        }

        [TestMethod]
        public void AddTenantTest()
        {
            var result = _catalogRepository.Add(SetTenant());
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetTenant()
        {
            var result = _catalogRepository.GetTenantAsync("tenantName");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetTenants()
        {
            var result = _catalogRepository.GetAllTenantsAsync();
            Assert.IsNotNull(result);
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
    }
}
