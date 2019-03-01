using BookMyHotel_Tenants.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookMyHotel.Tenants.Common.Interfaces
{
    public interface ITenantRepository
    {
        #region Cities

        Task<List<CityModel>> GetAllCities(int tenantId);
        Task<CityModel> GetCity(string cityCode, int tenantId);

        #endregion

        #region Customers

        Task<int> AddGuest(GuestModel guestModel, int tenantId);
        Task<GuestModel> GetGuest(string email, int tenantId);

        #endregion

        #region RoomPrices

        Task<List<RoomPriceModel>> GetRoomPrices(int roomId, int tenantId);

        #endregion

        #region Offers

        Task<List<OfferModel>> GetOffersForTenant(int tenantId);
        Task<OfferModel> GetOffer(int offerId, int tenantId);

        #endregion

        #region Rooms

        Task<List<RoomModel>> GetRooms(List<int> roomIds, int tenantId);
        Task<RoomModel> GetRoom(int roomId, int tenantId);

        #endregion

        #region BookingPurchases

        Task<int> AddBookinPurchase(BookingPurchaseModel bookingPurchaseModel, int tenantId);

        #endregion

        #region Bookings

        Task<bool> AddTickets(List<BookingModel> bookingModel, int tenantId);
        Task<int> GetBookingsSold(int sectionId, int eventId, int tenantId);

        #endregion

        #region Hotels

        Task<HotelModel> GetHotelDetails(int tenantId);

        #endregion

        #region HotelTypes

        Task<HotelTypeModel> GetHotelType(string hotelType, int tenantId);

        #endregion

    }
}
