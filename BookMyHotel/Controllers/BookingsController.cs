using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookMyHotel.Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace BookMyHotel.Controllers
{
    public class BookingsController : BaseController
    {
        #region Fields
        private readonly ITenantRepository _tenantRepository;
        private readonly ICatalogRepository _catalogRepository;
        private readonly ILogger _logger;
        private readonly DnsClient.ILookupClient _client;
        private readonly IConfiguration _configuration;
        private readonly IUtilities _utilities;
        private String _appRegion;
        #endregion

        public BookingsController(ITenantRepository tenantRepository, ICatalogRepository catalogRepository,
            IStringLocalizer<BaseController> baseLocalizer, ILogger<BookingsController> logger, IConfiguration configuration,
            DnsClient.ILookupClient client, IUtilities utilities)
            : base(baseLocalizer, tenantRepository, configuration, client)
        {
            _logger = logger;
            _tenantRepository = tenantRepository;
            _catalogRepository = catalogRepository;
            _client = client;
            _utilities = utilities;
            _configuration = configuration;
            _appRegion = configuration["APP_REGION"];
        }

        [Route("{tenant}")]
        public async Task<IActionResult> Index(string tenant)
        {
            try
            {
                if (!string.IsNullOrEmpty(tenant))
                {
                    var tenantDetails = await _catalogRepository.GetTenantAsync(tenant);
                    if (tenantDetails != null)
                    {
                        //Get tenant status
                        String tenantStatus = _utilities.GetTenantStatus(tenantDetails.TenantId);

                        if (tenantStatus == "Offline")
                        {
                            return View("TenantOffline", tenantDetails.TenantName);
                        }
                        else
                        {
                            var venueInfo = await _tenantRepository.GetHotelDetailsAsync(tenantDetails.TenantId);

                            //Get region of tenant database server using DNS
                            var dnsQuery = _client.Query(venueInfo.DatabaseServerName, DnsClient.QueryType.A);
                            String serverRegion = dnsQuery.Answers.ARecords().ElementAt(0).DomainName;

                            //Display bookings if tenant database is located in same region as app
                            if (serverRegion.Contains(_appRegion) && tenantStatus == "Online")
                            {
                                SetTenantConfig(tenantDetails.TenantId, tenantDetails.TenantIdInString);

                                var offers = await _tenantRepository.GetOffersForTenant(tenantDetails.TenantId);
                                return View(offers);
                            }
                            //Redirect to different app instance if tenant database is located in different region from app
                            else
                            {
                                var pairedRegion = (serverRegion.Split('-'))[0].Split('1')[0];
                                String recoveryAppInstance = "https://bookings-bookmyhotel-dpt-" + pairedRegion + "-" + _configuration["User"] + ".azurewebsites.net/" + tenant;
                                return Redirect(recoveryAppInstance);
                            }
                        }
                    }
                    else
                    {
                        return View("TenantError", tenant);
                    }
                }
            }
            catch (Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement.ShardManagementException ex)
            {
                if (ex.ErrorCode == Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement.ShardManagementErrorCode.MappingIsOffline)
                {
                    var tenantModel = await _catalogRepository.GetTenantAsync(tenant);
                    _logger.LogInformation(0, ex, "Tenant is offline: {tenant}", tenantModel.TenantName);
                    return View("TenantOffline", tenantModel.TenantName);
                }
                else if (ex.ErrorCode == Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement.ShardManagementErrorCode.MappingDoesNotExist)
                {
                    var tenantModel = await _catalogRepository.GetTenantAsync(tenant);
                    //Fix mapping irregularities
                    _utilities.ResolveMappingDifferences(tenantModel.TenantId);

                    //Get venue details
                    String tenantStatus = _utilities.GetTenantStatus(tenantModel.TenantId);
                    if (tenantStatus == "Online")
                    {
                        var offers = await _tenantRepository.GetOffersForTenant(tenantModel.TenantId);
                        return View(offers);
                    }
                    else
                    {
                        return View("TenantOffline", tenantModel.TenantName);
                    }
                }
                else
                {
                    _logger.LogError(0, ex, "Tenant shard was unavailable for tenant: {tenant}", tenant);
                    return View("Error", ex.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Get offers failed for tenant {tenant}", tenant);
                return View("Error", ex.Message);
            }
            return View("Error");
        }
    }
}