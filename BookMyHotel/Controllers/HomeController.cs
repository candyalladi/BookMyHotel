using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookMyHotel.Models;
using BookMyHotel_Tenants.Common.Interfaces;
using BookMyHotel.Tenants.Common.Interfaces;
using Microsoft.Extensions.Logging;
using BookMyHotel_Tenants.Common.Models;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;

namespace BookMyHotel.Controllers
{
    public class HomeController : Controller
    {
        #region Fields

        private readonly ICatalogRepository _catalogRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ILogger _logger;
        private readonly IUtilities _utilities;

        #endregion

        public HomeController(ICatalogRepository catalogRepository, ITenantRepository tenantRepository, ILogger<HomeController> logger, IUtilities utilities)
        {
            _catalogRepository = catalogRepository;
            _tenantRepository = tenantRepository;
            _logger = logger;
            _utilities = utilities;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var tenantsModel = await _catalogRepository.GetAllTenantsAsync();

                if (tenantsModel != null)
                {
                    //get the venue name for each tenant
                    foreach (var tenant in tenantsModel)
                    {
                        HotelModel venue = null;
                        String tenantStatus = _utilities.GetTenantStatus(tenant.TenantId);

                        if (tenantStatus == "Online")
                        {
                            try
                            {
                                venue = await _tenantRepository.GetHotelDetailsAsync(tenant.TenantId);
                            }
                            catch (ShardManagementException ex)
                            {
                                if (ex.ErrorCode == ShardManagementErrorCode.MappingDoesNotExist)
                                {
                                    //Fix mapping irregularities - trust local shard map
                                    _utilities.ResolveMappingDifferences(tenant.TenantId);

                                    //Get venue details if tenant is online
                                    String updatedTenantStatus = _utilities.GetTenantStatus(tenant.TenantId);
                                    if (updatedTenantStatus == "Online")
                                    {
                                        venue = await _tenantRepository.GetHotelDetailsAsync(tenant.TenantId);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(0, ex, "Error in getting all tenants in Bookings Hub");
                                return View("Error", ex.Message);
                            }
                        }

                        if (venue != null)
                        {
                            tenant.HotelName = venue.HotelName;
                            tenant.CityName = venue.CityCode;
                            tenant.TenantName = venue.DatabaseName;
                        }
                    }
                    return View(tenantsModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Error in getting all tenants in Events Hub");
                return View("Error", ex.Message);
            }
            return View("Error");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
