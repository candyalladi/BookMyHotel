using BookMyHotel.Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookMyHotelTests.BookMyHotel_Tenants.Common.Tests
{
    public class MockTenantRepository : ITenantRepository
    {
        #region Private Variables
        private List<CityModel> Cities { get; set; }
        private GuestModel CustomerModel { get; set; }
        #endregion

        #region Public Properties

        public List<RoomPriceModel> RoomPriceModels { get; set; }
        public List<RoomModel> RoomModels { get; set; }
        public List<BookingPurchaseModel> BookingPurchaseModels { get; set; }
        public List<BookingModel> BookingModels { get; set; }
        public List<GuestModel> GuestModels { get; set; }
        public List<OfferModel> OfferModels { get; set; }
        #endregion

        public MockTenantRepository()
        {
            var city = new CityModel
            {
                //Language = "en-us",
                CityCode = "HYD",
                CityName = "Hyderabad"
            };
            Cities = new List<CityModel> { city };

            RoomPriceModels = new List<RoomPriceModel>
            {
                new RoomPriceModel
                {
                    HotelId = 1,
                    RoomId = 1,
                    Price = 100
                },
                new RoomPriceModel
                {
                    HotelId = 2,
                    RoomId = 1,
                    Price = 80
                },
                new RoomPriceModel
                {
                    HotelId = 3,
                    RoomId = 1,
                    Price = 60
                }
            };

            RoomModels = new List<RoomModel>
            {
                new RoomModel
                {
                    RoomId = 1,
                    HotelId = 10,
                    RoomName = "section 1",
                    StandardPrice = 100,
                    RoomType = "Standard"
                },
                new RoomModel
                {
                    RoomId = 2,
                    HotelId = 20,
                    RoomName = "section 2",
                    StandardPrice = 80,
                    RoomType = "Luxury"
                }
            };

            BookingPurchaseModels = new List<BookingPurchaseModel>
            {
                new BookingPurchaseModel
                {
                    GuestId = 1,
                    TotalPrice = 2,
                    BookingPurchaseId = 5,
                    BookedDate = DateTime.Now
                }
            };

            BookingModels = new List<BookingModel>
            {
                new BookingModel
                {
                    RoomId = 1,
                    BookingId = 1,
                    BookingPurchaseId = 12,
                    RoomName = "Suite",
                    GuestId = 2,
                    HotelName = "Novatel",
                    Checkin_Date = DateTime.Now,
                    Checkout_Date = DateTime.Now.AddDays(2)


                }

            };

            OfferModels = new List<OfferModel>
            {
                new OfferModel
                {
                    OfferId = 1,
                    OfferDays = 3,
                    OfferValidTillDate = DateTime.Now.AddDays(3),
                    IsOfferAvailable = true,
                    HotelId = 1,
                    Discount = 25
                }
            };
        }

        public async Task<List<CityModel>> GetAllCitiesAsync(int tenantId)
        {
            return Cities;
        }

        public async Task<CityModel> GetCityAsync(string cityCode, int tenantId)
        {
            return Cities[0];
        }

        public async Task<int> AddGuestAsync(GuestModel guestModel, int tenantId)
        {
            CustomerModel = guestModel;
            return 123;
        }

        public async Task<GuestModel> GetGuest(string email, int tenantId)
        {
            return CustomerModel;
        }

        public async Task<List<RoomPriceModel>> GetRoomPricesAsync(int roomId, int tenantId)
        {
            return RoomPriceModels;
        }

        public async Task<List<OfferModel>> GetOffersForTenant(int tenantId)
        {
            return OfferModels;
        }

        public async Task<OfferModel> GetOffer(int offerId, int tenantId)
        {
            return OfferModels[0];
        }

        public async Task<List<RoomModel>> GetRoomsAsync(List<int> roomIds, int tenantId)
        {
            return RoomModels;
        }

        public async Task<RoomModel> GetRoomAsync(int roomId, int tenantId)
        {
            return RoomModels[0];
        }

        public async Task<int> AddBookinPurchase(BookingPurchaseModel bookingPurchaseModel, int tenantId)
        {
            BookingPurchaseModels.Add(bookingPurchaseModel);
            return bookingPurchaseModel.BookingPurchaseId;
        }

        public async Task<bool> AddBookings(List<BookingModel> bookingModels, int tenantId)
        {
            foreach (BookingModel bookingModel in bookingModels)
            {
                bookingModels.Add(bookingModel);
            }
            return true;
        }

        public async Task<int> GetBookingsSold(int sectionId, int eventId, int tenantId)
        {
            return BookingModels.Count;
        }

        public async Task<HotelModel> GetHotelDetailsAsync(int tenantId)
        {
            return new HotelModel
            {
                CityCode = "HYD",
                HotelType = "5 Star",
                HotelName = "Pramati",
                PostalCode = "123",
                AdminEmail = "admin@email.com",
                AdminPassword = "password"
            };
        }

        public async Task<HotelTypeModel> GetHotelTypeAsync(string hotelType, int tenantId)
        {
            return new HotelTypeModel
            {

                HotelType = "pop",
                RoomTypeShortNamePlural = "room short name",
                RoomTypeName = "Luxury",
                HotelTypeName = "5 Star",
                RoomTypeShortName = "short name"
            };
        }
    }
}
