using BookMyHotel_Tenants.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookMyHotel.Tenants.Common.Interfaces
{
    public interface ICatalogRepository
    {
        Task<List<TenantModel>> GetAllTenants();
        Task<TenantModel> GetTenant(string tenantName);
        bool Add(Tenants tenant);
    }
}
