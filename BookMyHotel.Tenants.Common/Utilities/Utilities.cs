﻿using BookMyHotel.Tenants.Common.Interfaces;
using BookMyHotel_Tenants.EmailService;
using BookMyHotel_Tenants.UserApp.EF.TenantsDbEFCore;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace BookMyHotel_Tenants.Common.Utilities
{
    /// <summary>
    /// The Utilities class for doing common methods
    /// </summary>
    public class Utilities : IUtilities
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public Utilities(IEmailService emailService, IConfiguration configuration)
        {
            _emailService = emailService;
            _configuration = configuration;
        }

        #region Public methods

        /// <summary>
        /// Register tenant shard
        /// </summary>
        /// <param name="tenantServerConfig">The tenant server configuration.</param>
        /// <param name="databaseConfig">The database configuration.</param>
        /// <param name="catalogConfig">The catalog configuration.</param>
        /// <param name="resetEventDate">If set to true, the events dates for all tenants will be reset </param>
        public async void RegisterTenantShard(TenantServerConfig tenantServerConfig, DatabaseConfig databaseConfig, CatalogConfig catalogConfig, bool resetEventDate)
        {
            //get all database in devtenantserver
            var tenants = GetAllTenantNames(tenantServerConfig, databaseConfig);

            var connectionString = new SqlConnectionStringBuilder
            {
                UserID = databaseConfig.DatabaseUser,
                Password = databaseConfig.DatabasePassword,
                ApplicationName = "EntityFramework",
                ConnectTimeout = databaseConfig.ConnectionTimeOut
            };

            foreach (var tenant in tenants)
            {
                var tenantId = GetTenantKey(tenant);
                var result = await Sharding.RegisterNewShard(tenant, tenantId, tenantServerConfig.TenantServer, databaseConfig.DatabaseServerPort, catalogConfig.ServicePlan);
                if (result)
                {
                    if (resetEventDate)
                    {
                        #region EF6
                        try
                        {
                            using (var context = new TenantContext(Sharding.ShardMap, tenantId, connectionString.ConnectionString))
                            {
                                context.Database.ExecuteSqlCommand("sp_ResetBookingDates");
                            }

                            var emailDetails = _configuration.GetSection("EmailNotification");
                            var fromEmailId = emailDetails["FromServiceEmailId"];
                            var ServiceName = emailDetails["ServiceName"];
                            _emailService.SendEmailToTenants(fromEmailId, ServiceName,"test@email.com","Pramati");
                        }
                        catch (ShardManagementException ex)
                        {
                            string errorText;
                            if (ex.ErrorCode == ShardManagementErrorCode.MappingIsOffline)
                                errorText = "Tenant '" + tenant + "' is offline. Could not reset booking dates:" + ex.ToString();
                            else
                                errorText = ex.ToString();
                            Console.WriteLine(errorText);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        #endregion
                       
                    }

                }
            }
        }

        /// <summary>
        /// Converts the int key to bytes array.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public byte[] ConvertIntKeyToBytesArray(int key)
        {
            byte[] normalized = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(key));

            // Maps Int32.Min - Int32.Max to UInt32.Min - UInt32.Max.
            normalized[0] ^= 0x80;

            return normalized;
        }

        /// <summary>
        /// Gets the status of the tenant mapping in the catalog.
        /// </summary>
        /// <param name="TenantId">The tenant identifier.</param>
        public String GetTenantStatus(int TenantId)
        {
            try
            {
                int mappingStatus = (int)Sharding.ShardMap.GetMappingForKey(TenantId).Status;

                if (mappingStatus > 0)
                    return "Online";
                else
                    return "Offline";
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Resolves any mapping differences between the global shard map in the catalog and the local shard map located a tenant database
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="UseGlobalShardMap">Specifies if the global shard map or the local shard map should be used as the source of truth for resolution.</param>
        public void ResolveMappingDifferences(int TenantId, bool UseGlobalShardMap = false)
        {
            Sharding.ResolveMappingDifferences(TenantId, UseGlobalShardMap);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets all tenant names from tenant server
        /// </summary>
        /// <param name="tenantServerConfig">The tenant server configuration.</param>
        /// <param name="databaseConfig">The database configuration.</param>
        /// <returns></returns>
        private List<string> GetAllTenantNames(TenantServerConfig tenantServerConfig, DatabaseConfig databaseConfig)
        {
            List<string> list = new List<string>();

            string conString = $"Server={databaseConfig.SqlProtocol}:{tenantServerConfig.TenantServer},{databaseConfig.DatabaseServerPort};" +
                $"Database={""};User ID={databaseConfig.DatabaseUser};Password={databaseConfig.DatabasePassword};" +
                $"Trusted_Connection=False;Encrypt=True;Connection Timeout={databaseConfig.ConnectionTimeOut};";

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases WHERE name NOT IN ('master')", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(dr[0].ToString());
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Generates the tenant Id using MD5 Hashing.
        /// </summary>
        /// <param name="tenantName">Name of the tenant.</param>
        /// <returns></returns>
        private int GetTenantKey(string tenantName)
        {
            var normalizedTenantName = tenantName.Replace(" ", string.Empty).ToLower();

            //Produce utf8 encoding of tenant name 
            var tenantNameBytes = Encoding.UTF8.GetBytes(normalizedTenantName);

            //Produce the md5 hash which reduces the size
            MD5 md5 = MD5.Create();
            var tenantHashBytes = md5.ComputeHash(tenantNameBytes);

            //Convert to integer for use as the key in the catalog 
            int tenantKey = BitConverter.ToInt32(tenantHashBytes, 0);

            return tenantKey;
        }
        #endregion
    }
}
