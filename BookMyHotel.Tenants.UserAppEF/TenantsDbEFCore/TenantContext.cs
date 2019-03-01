using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace BookMyHotel_Tenants.UserApp.EF.TenantsDbEFCore
{
    public partial class TenantContext : DbContext
    {
        public TenantContext(DbContextOptions<TenantContext> options)
            : base(options)
        {
        }

        public TenantContext(ShardMap shardMap, int shardingKey, string connectionStr)
            : base(CreateDdrConnection(shardMap, shardingKey, connectionStr))
        {

        }

        private static DbContextOptions CreateDdrConnection(ShardMap shardMap, int shardingKey, string connectionStr)
        {
            // Ask shard map to broker a validated connection for the given key
            SqlConnection sqlConn = shardMap.OpenConnectionForKey(shardingKey, connectionStr);

            var optionsBuilder = new DbContextOptionsBuilder<TenantContext>();
            var options = optionsBuilder.UseSqlServer(sqlConn).Options;

            return options;
        }

        public virtual DbSet<RoomsWithNoBookings> EventsWithNoTickets { get; set; }
        public virtual DbSet<database_firewall_rules> database_firewall_rules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<database_firewall_rules>()
                .Property(e => e.start_ip_address)
                .IsUnicode(false);

            modelBuilder.Entity<database_firewall_rules>()
                .Property(e => e.end_ip_address)
                .IsUnicode(false);
        }
    }
}
