using BookMyHotel_Tenants.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BookMyHotel_Tenants.UserApp.EF.CatalogDB;

namespace BookMyHotel_Tenants.Common.Interfaces
{
    public interface ICatalogRepository
    {
        Task<List<TenantModel>> GetAllTenantsAsync();
        Task<TenantModel> GetTenantAsync(string tenantName);
        bool Add(Tenants tenant);
    }
}
