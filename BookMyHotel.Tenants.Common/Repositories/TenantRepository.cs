using BookMyHotel.Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Mapper;
using BookMyHotel_Tenants.Common.Models;
using BookMyHotel_Tenants.Common.Utilities;
using BookMyHotel_Tenants.UserApp.EF.TenantsDB;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMyHotel_Tenants.Common.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        #region Private variables

        private readonly string _connectionString;

        #endregion

        public TenantRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Rooms/Bookings/Guests

        public async Task<int> AddBookingPurchase(BookingPurchaseModel bookingPurchaseModel, int tenantId)
        {
            using (var context = CreateContext(tenantId))
            {
                var bookingPurchase = bookingPurchaseModel.ToBookingPurchasesEntity();

                context.BookingPurchases.Add(bookingPurchase);
                await context.SaveChangesAsync();

                return bookingPurchase.BookingPurchaseId;
            }
        }

        public async Task<int> AddGuestAsync(GuestModel guestModel, int tenantId)
        {
            using (var context = CreateContext(tenantId))
            {
                var customer = guestModel.ToGuestsEntity();

                context.Guests.Add(customer);
                await context.SaveChangesAsync();

                return customer.GuestId;
            }
        }

        public async Task<bool> AddBookingsAsync(List<BookingModel> bookingModel, int tenantId)
        {
            using (var context = CreateContext(tenantId))
            {
                foreach (BookingModel ticketModel in bookingModel)
                {
                    context.Bookings.Add(ticketModel.ToBookingsEntity());
                }
                await context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<List<CityModel>> GetAllCitiesAsync(int tenantId)
        {
            using (var context = CreateContext(tenantId))
            {
                var allCountries = await context.Cities.ToListAsync();

                return allCountries.Count > 0 ? allCountries.Select(city => city.ToCityModel()).ToList() : null;
            }
        }

        public async Task<int> GetBookingsSold(int guestId, int eventId, int tenantId)
        {
            using (var context = CreateContext(tenantId))
            {
                var bookings = await context.BookingPurchases.Where(i => i.GuestId == guestId).ToListAsync();
                if (bookings.Any())
                {
                    return bookings.Count();
                }
            }
            return 0;
        }

        public async Task<CityModel> GetCityAsync(string cityCode, int tenantId)
        {
            using (var context = CreateContext(tenantId))
            {
                var country = await context.Cities.FirstOrDefaultAsync(x => x.CityCode == cityCode);
                return country?.ToCityModel();
            }
        }

        public async Task<GuestModel> GetGuest(string email, int tenantId)
        {
            using (var context = CreateContext(tenantId))
            {
                var customer = await context.Guests.FirstOrDefaultAsync(i => i.Email == email);

                return customer?.ToCustomerModel();
            }
        }

        public async Task<HotelModel> GetHotelDetailsAsync(int tenantId)
        {
            using (var context = CreateContext(tenantId))
            {
                //get database name
                string databaseName, databaseServerName;
                PointMapping<int> mapping;

                if (Sharding.ShardMap.TryGetMappingForKey(tenantId, out mapping))
                {
                    using (SqlConnection sqlConn = Sharding.ShardMap.OpenConnectionForKey(tenantId, _connectionString))
                    {
                        databaseName = sqlConn.Database;
                        databaseServerName = sqlConn.DataSource.Split(':').Last().Split(',').First();
                    }

                    var venue = await context.Hotels.FirstOrDefaultAsync();

                    if (venue != null)
                    {
                        var venueModel = venue.ToHotelModel();
                        venueModel.DatabaseName = databaseName;
                        venueModel.DatabaseServerName = databaseServerName;
                        return venueModel;
                    }
                }
                return null;
            }
        }

        public async Task<HotelTypeModel> GetHotelTypeAsync(string hotelType, int tenantId)
        {
            using (var context = CreateContext(tenantId))
            {
                var venueTypeDetails = await context.HotelTypes.FirstOrDefaultAsync(i => i.HotelType == hotelType);

                return venueTypeDetails?.ToHotelTypeModel();
            }
        }

        public async Task<OfferModel> GetOffer(int hotelId, int tenantId)
        {
            using (var context = CreateContext(tenantId))
            {
                var offer = await context.Offers.FirstOrDefaultAsync(i => i.HotelId == hotelId);
                return offer?.ToOfferModel();
            }
        }

        public async Task<List<OfferModel>> GetOffersForTenant(int tenantId)
        {
            using (var context = CreateContext(tenantId))
            {
                var offers = await context.Offers.ToListAsync();

                return offers.Count > 0 ? offers.Select(offer => offer.ToOfferModel()).ToList() : null;
            }
        }

        public async Task<RoomModel> GetRoomAsync(int roomId, int tenantId)
        {
            using (var context = CreateContext(tenantId))
            {
                var section = await context.Rooms.FirstOrDefaultAsync(i => i.RoomId == roomId);

                return section?.ToRoomModel();
            }
        }

        public async Task<List<RoomPriceModel>> GetRoomPricesAsync(int roomId, int tenantId)
        {
            using (var context = CreateContext(tenantId))
            {
                var roomPrices = await context.RoomPrices.ToListAsync();

                return roomPrices.Count > 0 ? roomPrices.Select(roomPrice => roomPrice.ToRoomPriceModel()).ToList() : null;
            }
        }

        public async Task<List<RoomModel>> GetRoomsAsync(List<int> roomIds, int tenantId)
        {
            using (var context = CreateContext(tenantId))
            {
                var sections = await context.Rooms.Where(i => roomIds.Contains(i.RoomId)).ToListAsync();

                return sections.Any() ? sections.Select(section => section.ToRoomModel()).ToList() : null;
            }
        }

        public async Task<bool> AddBookings(List<BookingModel> bookingModels, int tenantId)
        {
            using (var context = CreateContext(tenantId))
            {
                foreach (BookingModel bookingModel in bookingModels)
                {
                    context.Bookings.Add(bookingModel.ToBookingsEntity());
                }
                await context.SaveChangesAsync();
            }
            return true;
        }

        #endregion

        #region Venues

        public async Task<HotelModel> GetHotelDetails(int tenantId)
        {
            using (var context = CreateContext(tenantId))
            {
                //get database name
                string databaseName, databaseServerName;
                PointMapping<int> mapping;

                if (Sharding.ShardMap.TryGetMappingForKey(tenantId, out mapping))
                {
                    using (SqlConnection sqlConn = Sharding.ShardMap.OpenConnectionForKey(tenantId, _connectionString))
                    {
                        databaseName = sqlConn.Database;
                        databaseServerName = sqlConn.DataSource.Split(':').Last().Split(',').First();
                    }

                    var venue = await context.Hotels.FirstOrDefaultAsync();

                    if (venue != null)
                    {
                        var venueModel = venue.ToHotelModel();
                        venueModel.DatabaseName = databaseName;
                        venueModel.DatabaseServerName = databaseServerName;
                        return venueModel;
                    }
                }
                return null;
            }
        }

        #endregion

        #region VenueTypes

        public async Task<HotelTypeModel> GetHotelType(string hotelType, int tenantId)
        {
            using (var context = CreateContext(tenantId))
            {
                var venueTypeDetails = await context.HotelTypes.FirstOrDefaultAsync(i => i.HotelType == hotelType);

                return venueTypeDetails?.ToHotelTypeModel();
            }
        }

        #endregion

        #region Private methods

        private TenantDbContext CreateContext(int tenantId)
        {
            return new TenantDbContext(Sharding.ShardMap, tenantId, _connectionString);
        }
        
        #endregion
    }
}
