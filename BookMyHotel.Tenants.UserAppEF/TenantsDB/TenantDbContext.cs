using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace BookMyHotel_Tenants.UserApp.EF.TenantsDB
{
    public partial class TenantDbContext : DbContext
    {
        public virtual DbSet<Cities> Cities { get; set; }
        public virtual DbSet<Guests> Guests { get; set; }
        public virtual DbSet<RoomPrices> RoomPrices { get; set; }
        public virtual DbSet<Rooms> Rooms { get; set; }
        public virtual DbSet<BookingPurchases> BookingPurchases { get; set; }
        public virtual DbSet<Bookings> Tickets { get; set; }
        public virtual DbSet<Hotel> Hotel { get; set; }
        public virtual DbSet<HotelTypes> HotelTypes { get; set; }

        public TenantDbContext(ShardMap shardMap, int shardingKey, string connectionStr) :
            base(CreateDdrConnection(shardMap, shardingKey, connectionStr))
        {

        }

        /// <summary>
        /// Creates the DDR (Data Dependent Routing) connection.
        /// </summary>
        /// <param name="shardMap">The shard map.</param>
        /// <param name="shardingKey">The sharding key.</param>
        /// <param name="connectionStr">The connection string.</param>
        /// <returns></returns>
        private static DbContextOptions CreateDdrConnection(ShardMap shardMap, int shardingKey, string connectionStr)
        {
            // Ask shard map to broker a validated connection for the given key
            SqlConnection sqlConn = shardMap.OpenConnectionForKey(shardingKey, connectionStr);

            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            var options = optionsBuilder.UseSqlServer(sqlConn).Options;

            return options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cities>(entity =>
           {
               entity.HasKey(e => e.CityCode)
                   .HasName("PK__City__5D9B0D2D5E8496A7");

               entity.HasIndex(e => new { e.CityCode })
                   .HasName("IX_Countries_Country_Language")
                   .IsUnique();

               entity.Property(e => e.CityCode).HasColumnType("char(3)");

               entity.Property(e => e.CityName)
                   .IsRequired()
                   .HasMaxLength(50);
           });

            modelBuilder.Entity<Guests>(entity =>
            {
                entity.HasKey(e => e.GuestId)
                    .HasName("PK__Guest__A4AE64D814038057");

                entity.HasIndex(e => e.Email)
                    .HasName("IX_Customers_Email")
                    .IsUnique();

                entity.Property(e => e.GuestId)
                    .IsRequired()
                    .HasColumnType("char(3)");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.Property(e => e.Password).HasMaxLength(30);

                entity.Property(e => e.PostalCode).HasColumnType("char(10)");

                entity.Property(e => e.RowVersion).ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.CityCodeNavigation)
                    .WithMany(p => p.Guests)
                    .HasForeignKey(d => d.CityCode)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Guests_Cities");
            });

            modelBuilder.Entity<RoomPrices>(entity =>
            {
                entity.HasKey(e => new { e.RoomId, e.HotelId })
                    .HasName("PK__RoomId__414A3897F9A72D7B");

                entity.Property(e => e.Price).HasColumnType("money");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RoomPrices)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK_EventSections_Events");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RoomPrices)
                    .HasForeignKey(d => d.HotelId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_EventSections_Sections");
            });

            modelBuilder.Entity<Bookings>(entity =>
            {
                entity.HasKey(e => e.BookingId)
                    .HasName("PK__Booking__7944C81047DB4EF2");

                entity.Property(e => e.Checkin_Date).HasColumnType("datetime");
                entity.Property(e => e.Checkout_Date).HasColumnType("datetime");

                entity.Property(e => e.RoomId).HasColumnType("int");

                entity.Property(e => e.RoomName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.GuestId).HasColumnType("int");

                entity.Property(e => e.GuestName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.HotelName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Payment).HasColumnType("bool");
                entity.Property(e => e.BookingPurchaseId).HasColumnType("int");
            });

            modelBuilder.Entity<Rooms>(entity =>
            {
                entity.HasKey(e => e.RoomId)
                    .HasName("PK__Rooms__80EF0872FD27B716");

                entity.Property(e => e.HotelId).HasColumnType("int");

                entity.Property(e => e.RoomType).HasColumnType("char(3)");

                entity.Property(e => e.RoomName)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.StandardPrice)
                    .HasColumnType("money")
                    .HasDefaultValueSql("10");
            });

            modelBuilder.Entity<BookingPurchases>(entity =>
            {
                entity.HasKey(e => e.BookingPurchaseId)
                    .HasName("PK__BookingPu__97683DD692530887");

                entity.Property(e => e.BookingDate).HasColumnType("datetime");

                entity.Property(e => e.PurchaseTotal).HasColumnType("money");

                entity.Property(e => e.RowVersion).ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.Guest)
                    .WithMany(p => p.BookingPurchases)
                    .HasForeignKey(d => d.GuestId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_BookingPurchases_Customers");
            });

            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.HasKey(e => e.Lock)
                    .HasName("PK_Hotel");

                entity.Property(e => e.Lock)
                    .HasColumnType("char(1)")
                    .HasDefaultValueSql("'X'");

                entity.Property(e => e.AdminEmail)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.AdminPassword).HasColumnType("nchar(30)");

                entity.Property(e => e.CityCode)
                    .IsRequired()
                    .HasColumnType("char(3)");

                entity.Property(e => e.PostalCode).HasColumnType("char(10)");

                entity.Property(e => e.HotelName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.HotelType)
                    .IsRequired()
                    .HasColumnType("char(30)");

                entity.HasOne(d => d.CityCodeNavigation)
                    .WithMany(p => p.Hotel)
                    .HasForeignKey(d => d.CityCode)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Hotel_Cities");

                entity.HasOne(d => d.HotelTypeNavigation)
                    .WithMany(p => p.Hotel)
                    .HasForeignKey(d => d.HotelType)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Venues_VenueTypes");
            });

            modelBuilder.Entity<HotelTypes>(entity =>
            {
                entity.HasKey(e => e.HotelType)
                    .HasName("PK__HotelTyp__265E44FD9586CE48");

                entity.HasIndex(e => new { e.HotelTypeName })
                    .IsUnique();

                entity.Property(e => e.HotelType).HasColumnType("char(30)");

                entity.Property(e => e.RoomTypeName)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.RoomTypeShortName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.RoomTypeShortNamePlural)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.HotelTypeName)
                    .IsRequired()
                    .HasColumnType("nchar(30)");
            });
        }

    }
}
