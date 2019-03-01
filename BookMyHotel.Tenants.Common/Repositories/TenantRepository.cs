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

        public Task<int> AddBookinPurchase(BookingPurchaseModel bookingPurchaseModel, int tenantId)
        {
            throw new NotImplementedException();
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
                    context.Tickets.Add(ticketModel.ToBookingsEntity());
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

        public Task<int> GetBookingsSold(int sectionId, int eventId, int tenantId)
        {
            throw new NotImplementedException();
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

                    var venue = await context.Hotel.FirstOrDefaultAsync();

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

        public Task<OfferModel> GetOffer(int offerId, int tenantId)
        {
            throw new NotImplementedException();
        }

        public Task<List<OfferModel>> GetOffersForTenant(int tenantId)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public async Task<List<RoomModel>> GetRoomsAsync(List<int> roomIds, int tenantId)
        {
            using (var context = CreateContext(tenantId))
            {
                var sections = await context.Rooms.Where(i => roomIds.Contains(i.RoomId)).ToListAsync();

                return sections.Any() ? sections.Select(section => section.ToRoomModel()).ToList() : null;
            }
        }

        #region Private methods

        private TenantDbContext CreateContext(int tenantId)
        {
            return new TenantDbContext(Sharding.ShardMap, tenantId, _connectionString);
        }
        #endregion
    }
}
