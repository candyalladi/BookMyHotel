using BookMyHotel.Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Mapper;
using BookMyHotel_Tenants.Common.Models;
using BookMyHotel_Tenants.UserApp.EF.CatalogDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BookMyHotel_Tenants.Common.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly CatalogDbContext _catalogDbContext;
        private readonly IConfiguration _configuration;

        public CatalogRepository(CatalogDbContext catalogDbContext, IConfiguration configuration)
        {
            _catalogDbContext = catalogDbContext;
            _configuration = configuration;
        }

        public bool Add(Tenants tenant)
        {
            var bookMyHotelUser = _configuration["User"];
            var tenantServerName = "tenants1-dpt-" + bookMyHotelUser;
            var normalizedTenantName = tenant.TenantName.Replace(" ", string.Empty).ToLower();
            var location = _configuration["APP_REGION"];
            var defaultTenantValues = _configuration.GetSection("DefaultEnvironment");

            //Add tenant to tenants table
            _catalogDbContext.Tenants.Add(tenant);

            //Add tenant database resources to catalog
            var tenantDatabase = new Databases
            {
                ServerName = tenantServerName,
                DatabaseName = normalizedTenantName,
                ServiceObjective = defaultTenantValues["DatabaseServiceObjective"],
                ElasticPoolName = defaultTenantValues["ElasticPoolName"],
                State = "created",
                RecoveryState = "n/a",
                LastUpdated = System.DateTime.Now
            };
            var databaseExists = (from a in _catalogDbContext.Databases where a.DatabaseName == tenantDatabase.DatabaseName && a.ServerName == tenantDatabase.ServerName select a);

            if (databaseExists.FirstOrDefault() == null)
            {
                _catalogDbContext.Databases.Add(tenantDatabase);
            }


            //Add tenant elastic pool resources to catalog
            var tenantElasticPool = new ElasticPools
            {
                ServerName = tenantServerName,
                ElasticPoolName = defaultTenantValues["ElasticPoolName"],
                Dtu = Int32.Parse(defaultTenantValues["ElasticPoolDTU"]),
                Edition = defaultTenantValues["ElasticPoolEdition"],
                DatabaseDtuMax = Int32.Parse(defaultTenantValues["ElasticPoolDatabaseDtuMax"]),
                DatabaseDtuMin = Int32.Parse(defaultTenantValues["ElasticPoolDatabaseDtuMin"]),
                StorageMB = Int32.Parse(defaultTenantValues["ElasticPoolStorageMB"]),
                State = "created",
                RecoveryState = "n/a",
                LastUpdated = System.DateTime.Now
            };
            var poolExists = (from a in _catalogDbContext.ElasticPools where a.ElasticPoolName == tenantElasticPool.ElasticPoolName && a.ServerName == tenantElasticPool.ServerName select a);

            if (poolExists.FirstOrDefault() == null)
            {
                _catalogDbContext.ElasticPools.Add(tenantElasticPool);
            }


            //Add tenant server resources to catalog
            var tenantServer = new Servers
            {
                ServerName = tenantServerName,
                Location = location,
                State = "created",
                RecoveryState = "n/a",
                LastUpdated = System.DateTime.Now
            };
            var serverExists = (from a in _catalogDbContext.Servers where a.ServerName == tenantServer.ServerName select a);

            if (serverExists.FirstOrDefault() == null)
            {
                _catalogDbContext.Servers.Add(tenantServer);
            }

            _catalogDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<TenantModel>> GetAllTenantsAsync()
        {
            var allTenantsList = await _catalogDbContext.Tenants.ToListAsync();

            if (allTenantsList.Count > 0)
            {
                return allTenantsList.Select(tenant => tenant.ToTenantModel()).ToList();
            }

            return null;
        }

        public async Task<TenantModel> GetTenantAsync(string tenantName)
        {
            var tenants = await _catalogDbContext.Tenants.Where(i => Regex.Replace(i.TenantName.ToLower(), @"\s+", "") == tenantName).ToListAsync();

            if (tenants.Any())
            {
                var tenant = tenants.FirstOrDefault();
                return tenant?.ToTenantModel();
            }

            return null;
        }
    }
}
