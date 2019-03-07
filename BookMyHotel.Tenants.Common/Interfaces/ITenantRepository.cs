using BookMyHotel_Tenants.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookMyHotel.Tenants.Common.Interfaces
{
    public interface ITenantRepository
    {
        #region Cities

        Task<List<CityModel>> GetAllCitiesAsync(int tenantId);
        Task<CityModel> GetCityAsync(string cityCode, int tenantId);

        #endregion

        #region Guests

        Task<int> AddGuestAsync(GuestModel guestModel, int tenantId);
        Task<GuestModel> GetGuest(string email, int tenantId);

        #endregion

        #region RoomPrices

        Task<List<RoomPriceModel>> GetRoomPricesAsync(int roomId, int tenantId);

        #endregion

        #region Offers

        Task<List<OfferModel>> GetOffersForTenant(int tenantId);
        Task<OfferModel> GetOffer(int offerId, int tenantId);

        #endregion

        #region Rooms

        Task<List<RoomModel>> GetRoomsAsync(List<int> roomIds, int tenantId);
        Task<RoomModel> GetRoomAsync(int roomId, int tenantId);

        #endregion

        #region BookingPurchases

        Task<int> AddBookinPurchase(BookingPurchaseModel bookingPurchaseModel, int tenantId);

        #endregion

        #region Bookings

        Task<bool> AddBookings(List<BookingModel> bookingModel, int tenantId);
        Task<int> GetBookingsSold(int sectionId, int eventId, int tenantId);

        #endregion

        #region Hotels

        Task<HotelModel> GetHotelDetailsAsync(int tenantId);

        #endregion

        #region HotelTypes

        Task<HotelTypeModel> GetHotelTypeAsync(string hotelType, int tenantId);

        #endregion

    }
}
