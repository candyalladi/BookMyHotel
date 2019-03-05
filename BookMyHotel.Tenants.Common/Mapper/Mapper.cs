using BookMyHotel_Tenants.Common.Models;
using BookMyHotel_Tenants.UserApp.EF.CatalogDB;
using BookMyHotel_Tenants.UserApp.EF.TenantsDB;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BookMyHotel_Tenants.Common.Mapper
{
    public static class Mapper
    {
        #region Entity To Model Mapping

        public static TenantModel ToTenantModel(this Tenants tenantEntity)
        {
            string tenantIdInString = BitConverter.ToString(tenantEntity.TenantId);
            tenantIdInString = tenantIdInString.Replace("-", "");

            return new TenantModel
            {
                ServicePlan = tenantEntity.ServicePlan,
                TenantId = ConvertByteKeyIntoInt(tenantEntity.TenantId),
                TenantName = tenantEntity.TenantName,
                TenantIdInString = tenantIdInString,
                RecoveryState = tenantEntity.RecoveryState,
                LastUpdated = tenantEntity.LastUpdated
            };
        }

        public static CityModel ToCityModel(this Cities city)
        {
            return new CityModel
            {
                CityCode = city.CityCode.Trim(),
                CityName = city.CityName.Trim()
            };
        }

        public static HotelModel ToHotelModel(this Hotels hotelModel)
        {
            return new HotelModel
            {
                HotelId = hotelModel.HotelId,
                HotelName = hotelModel.HotelName.Trim(),
                AdminEmail = hotelModel.AdminEmail.Trim(),
                AdminPassword = hotelModel.AdminPassword,
                CityCode = hotelModel.CityCode.Trim(),
                PostalCode = hotelModel.PostalCode,
                HotelType = hotelModel.HotelType.Trim()
            };
        }

        public static HotelTypeModel ToHotelTypeModel(this HotelTypes hotelType)
        {
            return new HotelTypeModel
            {
                HotelType = hotelType.HotelType.Trim(),
                RoomTypeName = hotelType.RoomTypeName.Trim(),
                RoomTypeShortName = hotelType.RoomTypeShortName.Trim(),
                RoomTypeShortNamePlural = hotelType.RoomTypeShortNamePlural.Trim(),
                HotelTypeName = hotelType.HotelTypeName.Trim()
            };
        }

        public static GuestModel ToCustomerModel(this Guests customer)
        {
            return new GuestModel
            {
                FirstName = customer.FirstName,
                Email = customer.Email,
                PostalCode = customer.PostalCode,
                LastName = customer.LastName,
                CityCode = customer.CityCode,
                GuestId = customer.GuestId,
                RoomId = customer.RoomId,
                TenantName = customer.TenantName
            };
        }

        public static RoomPriceModel ToRoomPriceModel(this RoomPrices roomPrices)
        {
            return new RoomPriceModel
            {
                RoomId = roomPrices.RoomId,
                Price = roomPrices.Price,
                HotelId = roomPrices.HotelId
            };
        }

        public static BookingModel ToBookingModel(this Bookings bookingsEntity)
        {
            return new BookingModel
            {
                Checkin_Date = bookingsEntity.Checkin_Date,
                Checkout_Date = bookingsEntity.Checkout_Date,
                RoomId = bookingsEntity.RoomId,
                RoomName = bookingsEntity.RoomName.Trim(),
                BookingId = bookingsEntity.BookingId,
                GuestId = bookingsEntity.GuestId,
                GuestName = bookingsEntity.GuestName,
                Payment = bookingsEntity.Payment,
                HotelName = bookingsEntity.HotelName,
                BookingPurchaseId = bookingsEntity.BookingPurchaseId
            };
        }

        public static RoomModel ToRoomModel(this Rooms rooms)
        {
            return new RoomModel
            {
                RoomId = rooms.RoomId,
                HotelId = rooms.HotelId,
                RoomName = rooms.RoomName,
                //RoomType = rooms.RoomType, Need to check conversion
                StandardPrice = rooms.StandardPrice
            };
        }

        public static OfferModel ToOfferModel(this Offers offers)
        {
            return new OfferModel
            {
                OfferId = offers.OfferId,
                OfferValidTillDate = offers.OfferValidTillDate,
                Discount = offers.Discount,
                HotelId = offers.HotelId,
                IsOfferAvailable = offers.IsOfferAvailable,
                OfferDays = offers.OfferDays
            };
        }

        public static BookingPurchaseModel ToBookingPurchaseModel(this BookingPurchaseModel bookingPurchaseModel)
        {
            return new BookingPurchaseModel
            {
                BookingPurchaseId = bookingPurchaseModel.BookingPurchaseId,
                BookedDate = bookingPurchaseModel.BookedDate,
                TotalPrice = bookingPurchaseModel.TotalPrice,
                GuestId = bookingPurchaseModel.GuestId,
                OfferId = bookingPurchaseModel.OfferId
            };
        }

        #endregion

        #region Model to Entity Mapping

        public static Cities ToCitiesEntity(this CityModel cityModel)
        {
            return new Cities
            {
                CityCode = cityModel.CityCode,
                CityName = cityModel.CityName
            };
        }

        public static Hotels ToHotelEntity(this HotelModel hotelModel)
        {
            return new Hotels
            {
                HotelId = hotelModel.HotelId,
                HotelName = hotelModel.HotelName,
                AdminEmail = hotelModel.AdminEmail,
                AdminPassword = hotelModel.AdminPassword,
                PostalCode = hotelModel.PostalCode,
                CityCode = hotelModel.CityCode,
                HotelType = hotelModel.CityCode
            };
        }

        public static HotelTypes ToHotelTypesEntity(this HotelTypeModel hotelTypeModel)
        {
            return new HotelTypes
            {
                HotelType = hotelTypeModel.HotelType,
                HotelTypeName = hotelTypeModel.HotelTypeName,
                RoomTypeName = hotelTypeModel.RoomTypeName,
                RoomTypeShortName = hotelTypeModel.RoomTypeShortName,
                RoomTypeShortNamePlural = hotelTypeModel.RoomTypeShortNamePlural
            };
        }

        public static Rooms ToRoomsEntity(this RoomModel roomModel)
        {
            return new Rooms
            {
                RoomId = roomModel.RoomId,
                RoomName = roomModel.RoomName,
                RoomType = roomModel.RoomType,
                HotelId = roomModel.HotelId,
                StandardPrice = roomModel.StandardPrice
            };
        }

        public static RoomPrices ToRoomPricesEntity(this RoomPriceModel roomPriceModel)
        {
            return new RoomPrices
            {
                HotelId = roomPriceModel.HotelId,
                RoomId = roomPriceModel.RoomId,
                Price = roomPriceModel.Price
            };
        }

        public static Guests ToGuestsEntity(this GuestModel customeModel)
        {
            return new Guests
            {
                CityCode = customeModel.CityCode,
                Email = customeModel.Email,
                FirstName = customeModel.FirstName,
                LastName = customeModel.LastName,
                PostalCode = customeModel.PostalCode,
                GuestId = customeModel.GuestId,
                RoomId = customeModel.RoomId,
                TenantName = customeModel.TenantName
            };
        }

        public static BookingPurchases ToBookingPurchasesEntity(this BookingPurchaseModel ticketPurchaseModel)
        {
            //password not required to save demo friction
            return new BookingPurchases
            {
                GuestId = ticketPurchaseModel.GuestId,
                BookingDate = DateTime.Now,
                PurchaseTotal = ticketPurchaseModel.TotalPrice,
                BookingPurchaseId = ticketPurchaseModel.BookingPurchaseId
            };
        }

        public static Bookings ToBookingsEntity(this BookingModel bookingModel)
        {
            return new Bookings
            {
                BookingId = bookingModel.BookingId,
                BookingPurchaseId = bookingModel.BookingPurchaseId,
                RoomId = bookingModel.RoomId,
                GuestId = bookingModel.GuestId,
                Checkin_Date = bookingModel.Checkin_Date,
                Checkout_Date = bookingModel.Checkout_Date,
                RoomName = bookingModel.RoomName,
                GuestName = bookingModel.GuestName,
                Payment = bookingModel.Payment,
                HotelName = bookingModel.HotelName
            };
        }

        public static Offers ToOffersEntity(this OfferModel offerModel)
        {
            return new Offers
            {
                OfferId = offerModel.OfferId,
                OfferValidTillDate = offerModel.OfferValidTillDate,
                Discount = offerModel.Discount,
                HotelId = offerModel.HotelId,
                IsOfferAvailable = offerModel.IsOfferAvailable,
                OfferDays = offerModel.OfferDays
            };
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Converts the byte key into int.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private static int ConvertByteKeyIntoInt(byte[] key)
        {
            // Make a copy of the normalized array
            byte[] denormalized = new byte[key.Length];

            key.CopyTo(denormalized, 0);

            // Flip the last bit and cast it to an integer
            denormalized[0] ^= 0x80;

            return IPAddress.HostToNetworkOrder(BitConverter.ToInt32(denormalized, 0));
        }

        #endregion
    }
}
