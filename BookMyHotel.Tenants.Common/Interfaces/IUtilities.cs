using BookMyHotel_Tenants.Common.Utilities;

namespace BookMyHotel.Tenants.Common.Interfaces
{
    public interface IUtilities
    {
        void RegisterTenantShard(TenantServerConfig tenantServerConfig, DatabaseConfig databaseConfig, CatalogConfig catalogConfig, bool resetBookingDate);

        byte[] ConvertIntKeyToBytesArray(int key);

        string GetTenantStatus(int TenantId);

        void ResolveMappingDifferences(int TenantId, bool UseGlobalShardMap = false);
    }
}
