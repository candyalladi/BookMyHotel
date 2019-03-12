using BookMyHotel.Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Utilities;
using BookMyHotel_Tenants.UserApp.EF.CatalogDB;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Xunit;
using Assert = Xunit.Assert;

namespace BookMyHotelTests.BookMyHotel_Tenants.Common.Tests.ShardingTests
{
    [TestClass]
    public class ShardingTests
    {
        #region Private fields

        internal const string TestServer = @"PRINHYLTPDL1249\SQLEXPRESS";
        internal const string ShardMapManagerTestConnectionString = "Data Source=" + TestServer + ";Integrated Security=True;";

        private const string CreateDatabaseQueryFormat =
            "IF EXISTS (SELECT name FROM sys.databases WHERE name = N'{0}') BEGIN DROP DATABASE [{0}] END CREATE DATABASE [{0}]";

        private CatalogConfig _catalogConfig;
        private DatabaseConfig _databaseConfig;
        private string _connectionString;

        private MockCatalogRepository _mockCatalogRepo;
        private MockTenantRepository _mockTenantRepo;
        private Mock<IUtilities> _mockUtilities;

        #endregion

        public ShardingTests()
        {
            _catalogConfig = new CatalogConfig
            {
                ServicePlan = "Standard",
                CatalogDatabase = "ShardMapManager",
                CatalogServer = TestServer
            };

            _databaseConfig = new DatabaseConfig
            {
                DatabasePassword = "",
                DatabaseUser = "",
                ConnectionTimeOut = 30,
                DatabaseServerPort = 1433,
                LearnHowFooterUrl = "",
                SqlProtocol = SqlProtocol.Tcp
            };

            var tenant = new Tenants
            {
                ServicePlan = "Standard",
                TenantName = "TestTenant",
                TenantId = new byte[0]
            };

            _connectionString = string.Format("{0}Initial Catalog={1};", ShardMapManagerTestConnectionString, _catalogConfig.CatalogDatabase);

            _mockCatalogRepo = new MockCatalogRepository();
            _mockCatalogRepo.Add(tenant);

            _mockTenantRepo = new MockTenantRepository();

            _mockUtilities = new Mock<IUtilities>();

            #region Create databases on localhost

            // Clear all connection pools.
            SqlConnection.ClearAllPools();

            using (SqlConnection conn = new SqlConnection(ShardMapManagerTestConnectionString))
            {
                conn.Open();

                // Create ShardMapManager database
                using (SqlCommand cmd = new SqlCommand(string.Format(CreateDatabaseQueryFormat, _catalogConfig.CatalogDatabase), conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Create Tenant database
                using (SqlCommand cmd = new SqlCommand(string.Format(CreateDatabaseQueryFormat, tenant.TenantName), conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            #endregion
        }
        
        [Fact]
        public void ShardingTest()
        {
            var sharding = new Sharding(_catalogConfig.CatalogDatabase, _connectionString, _mockCatalogRepo, _mockTenantRepo, _mockUtilities.Object);

            Assert.NotNull(sharding);
            Assert.NotNull(sharding.ShardMapManager);
        }

        [Fact]
        public async void RegisterShardTest()
        {
            _databaseConfig = new DatabaseConfig
            {
                SqlProtocol = SqlProtocol.Default
            };

            var sharding = new Sharding(_catalogConfig.CatalogDatabase, _connectionString, _mockCatalogRepo, _mockTenantRepo, _mockUtilities.Object);
            var result = await Sharding.RegisterNewShard("TestTenant", 1, TestServer, 1433, _catalogConfig.ServicePlan);

            Assert.True(result);
        }
    }
}
