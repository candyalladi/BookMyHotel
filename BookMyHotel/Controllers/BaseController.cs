using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookMyHotel.Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Models;
using BookMyHotel_Tenants.Common.Utilities;
using DnsClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace BookMyHotel.Controllers
{
    public class BaseController : Controller
    {
        #region Fields
        private readonly IStringLocalizer<BaseController> _localizer;
        private readonly ITenantRepository _tenantRepository;
        private readonly IConfiguration _configuration;
        private readonly ILookupClient _client;
        #endregion

        public BaseController(IStringLocalizer<BaseController> localizer, ITenantRepository tenantRepository,
            IConfiguration configuration, DnsClient.ILookupClient client)
        {
            _localizer = localizer;
            _tenantRepository = tenantRepository;
            _configuration = configuration;
            _client = client;
        }

        #region Protected Methods

        protected void DisplayMessage(string content, string header)
        {
            if (!string.IsNullOrWhiteSpace(content))
            {
                string heading = header == "Confirmation" ? _localizer["Confirmation"] : _localizer["Error"];

                TempData["msg"] = $"<script>showAlert(\'{heading}\', '{content}');</script>";
            }
        }

        protected void SetTenantConfig(int tenantId, string tenantIdInString)
        {
            var host = HttpContext.Request.Host.ToString();

            var tenantConfig = PopulateTenantConfigs(tenantId, tenantIdInString, host);

            if (tenantConfig != null)
            {
                var tenantConfigs = HttpContext.Session.GetObjectFromJson<List<TenantConfig>>("TenantConfigs");
                if (tenantConfigs == null)
                {
                    tenantConfigs = new List<TenantConfig>
                    {
                        tenantConfig
                    };
                    HttpContext.Session.SetObjectAsJson("TenantConfigs", tenantConfigs);
                }
                else
                {
                    var tenantsInfo = tenantConfigs.Where(i => i.TenantId == tenantId);

                    if (!tenantsInfo.Any())
                    {
                        tenantConfigs.Add(tenantConfig);
                        HttpContext.Session.SetObjectAsJson("TenantConfigs", tenantConfigs);
                    }
                    else
                    {
                        for (var i = 0; i < tenantConfigs.Count; i++)
                        {
                            if (tenantConfigs[i].TenantId == tenantId)
                            {
                                tenantConfigs[i] = tenantConfig;
                                HttpContext.Session.SetObjectAsJson("TenantConfigs", tenantConfigs);
                                break;
                            }
                        }
                    }
                }

            }
        }

        /// <summary>
        /// This method will return the tickets model that will be used for the database inserts
        /// </summary>
        /// <param name="bookingId">The tenant identifier.</param>
        /// <param name="roomId">Section Id for the tickets.</param>
        /// <param name="numberOfRooms">Count of tickets.</param>
        /// <param name="bookingPurchaseId">Parent id for which the tickets should be tied to</param>
        /// <returns></returns>
        protected List<BookingModel> BuildTicketModel(int bookingId, int roomId, int numberOfRooms, int bookingPurchaseId)
        {
            var ticketsModel = new List<BookingModel>();
            for (var i = 0; i < numberOfRooms; i++)
            {
                ticketsModel.Add(new BookingModel
                {
                    RoomId = roomId,
                    BookingId = bookingId,
                    BookingPurchaseId = bookingPurchaseId,
                    RoomName = $"{roomId}{bookingId}{bookingPurchaseId}", // ensures that the room name is always unique
                    GuestId = i + 1
                });
            }
            return ticketsModel;
        }

        /// <summary>
        /// Populates the tenant configs.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="tenantIdInString">The tenant identifier in string.</param>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        private TenantConfig PopulateTenantConfigs(int tenantId, string tenantIdInString, string host)
        {
            try
            {
                //get blobPath
                var blobPath = _configuration["BlobPath"];
                var defaultCulture = _configuration["DefaultRequestCulture"];

                //get user from url
                string user;
                if (host.Contains("localhost"))
                {
                    user = "testuser";
                }
                else
                {
                    string[] hostpieces = host.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    user = hostpieces[2];
                }

                //get the venue details and populate in config settings
                var venueDetails = (_tenantRepository.GetHotelDetailsAsync(tenantId)).Result;
                var venueTypeDetails = (_tenantRepository.GetHotelTypeAsync(venueDetails.HotelType, tenantId)).Result;
                var countries = (_tenantRepository.GetAllCitiesAsync(tenantId)).Result;
                var tenantServerName = venueDetails.DatabaseServerName;

                //get country language from db 
                var country = (_tenantRepository.GetCityAsync(venueDetails.CityCode, tenantId)).Result;
                RegionInfo regionalInfo = new RegionInfo(defaultCulture);

                return new TenantConfig
                {
                    DatabaseName = venueDetails.DatabaseName,
                    DatabaseServerName = tenantServerName,
                    HotelName = venueDetails.HotelName,
                    BlobImagePath = blobPath + venueTypeDetails.HotelType + "-user.jpg",
                    HotelTypeNamePlural = venueTypeDetails.RoomTypeShortNamePlural.ToUpper(),
                    TenantId = tenantId,
                    TenantName = venueDetails.DatabaseName,
                    Currency = regionalInfo.CurrencySymbol,
                    TenantCulture = defaultCulture,
                    TenantCities = countries,
                    TenantIdInString = tenantIdInString,
                    User = user
                };
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.Message, "Error in populating tenant config.");
            }
            return null;
        }


        #endregion
    }
}