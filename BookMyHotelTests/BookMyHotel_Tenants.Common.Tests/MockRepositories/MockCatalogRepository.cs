using BookMyHotel_Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Models;
using BookMyHotel_Tenants.UserApp.EF.CatalogDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMyHotelTests.BookMyHotel_Tenants.Common.Tests
{
    public class MockCatalogRepository : ICatalogRepository
    {
        private List<Tenants> Tenants { get; set; }

        public MockCatalogRepository()
        {
            Tenants = new List<Tenants>();
        }

        public async Task<List<TenantModel>> GetAllTenantsAsync()
        {
            return Tenants.Select(tenant => new TenantModel
            {
                TenantId = BitConverter.ToInt32(tenant.TenantId, 0),
                TenantName = tenant.TenantName,
                ServicePlan = tenant.ServicePlan
            }).ToList();
        }

        public async Task<TenantModel> GetTenantAsync(string tenantName)
        {
            var tenant = Tenants[0];
            TenantModel tenantModel = new TenantModel
            {
                TenantId = BitConverter.ToInt32(tenant.TenantId, 0),
                TenantName = tenant.TenantName,
                ServicePlan = tenant.ServicePlan
            };
            return tenantModel;
        }

        public bool Add(Tenants tenant)
        {
            Tenants.Add(tenant);
            return true;
        }
    }
}
