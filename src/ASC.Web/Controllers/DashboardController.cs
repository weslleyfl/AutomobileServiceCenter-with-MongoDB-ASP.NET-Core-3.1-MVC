using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Business.Interfaces;
using ASC.Models.BaseTypes;
using ASC.Models.Models;
using ASC.Utilities;
using ASC.Web.Configuration;
using ASC.Web.Controllers.Base;
using ASC.Web.Data.Cache;
using ASC.Web.Models.ServiceRequestViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ASC.Web.Controllers
{
    /// <summary> 
    /// REGRAS DE NEGOCIO
    /// 1. For a Customer
    ///    a.It should provide list of his own service requests during the past one year
    ///   sorted from latest to oldest.
    /// 2. For a Service Engineer
    ///    a.It should provide list of his associated on-going service requests in
    ///        last 7 days which are holding status of New, Initiated, InProgress
    ///        and RequestForInformation and finally sorted by Requested Date in
    ///        descending order.
    ///    b.It should also provide the top 20 recent service updates of the service
    ///        requests associated with him.
    /// 3. For an Admin
    ///    a. It should provide list of on-going service requests during the last week which
    ///        are holding status of New, Initiated, InProgress and RequestForInformation
    ///        sorted by Requested Date in descending order.
    ///    b.It should also provide the top 20 recent updates across all service requests.
    ///    c.It should provide statistics about service engineer to his associated on-going service requests
    /// </summary>
    public class DashboardController : BaseController
    {
        private readonly IOptions<ApplicationSettings> _settings;
        private readonly IServiceRequestOperations _serviceRequestOperations;
        private readonly IMasterDataCacheOperations _masterData;

        public DashboardController(IOptions<ApplicationSettings> settings,
            IServiceRequestOperations serviceRequestOperations,
            IMasterDataCacheOperations masterData)
        {
            _settings = settings;
            _serviceRequestOperations = serviceRequestOperations;
            _masterData = masterData;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {

            var status = new List<string>()
            {
                Status.New.ToString(),
                Status.InProgress.ToString(),
                Status.Initiated.ToString(),
                Status.RequestForInformation.ToString()
            };

            List<ServiceRequest> serviceRequests = new List<ServiceRequest>();
            Dictionary<string, int> activeServiceRequests = new Dictionary<string, int>();

            if (HttpContext.User.IsInRole(Roles.Admin.ToString()))
            {
                var results = await _serviceRequestOperations
                    .GetServiceRequestsByRequestedDateAndStatusAsync(
                         requestedDate: DateTime.UtcNow.AddDays(-7),
                         status: status);

                var serviceEngineerServiceRequests = await _serviceRequestOperations
                    .GetActiveServiceRequestsAsync(new List<string>()
                     {
                       Status.Initiated.ToString(),
                       Status.InProgress.ToString()
                     });

                if (serviceEngineerServiceRequests.Any())
                {
                    activeServiceRequests = serviceEngineerServiceRequests
                        .GroupBy(s => s.ServiceEngineer)
                        .ToDictionary(p => p.Key, p => p.Count());
                }

                serviceRequests = results.ToList();
            }
            else if (HttpContext.User.IsInRole(Roles.Engineer.ToString()))
            {
                var results = await _serviceRequestOperations
                    .GetServiceRequestsByRequestedDateAndStatusAsync(
                        DateTime.UtcNow.AddDays(-7),
                        status,
                        serviceEngineerEmail: HttpContext.User.GetCurrentUserDetails().Email);

                serviceRequests = results.ToList();
            }
            else
            {
                var results = await _serviceRequestOperations
                    .GetServiceRequestsByRequestedDateAndStatusAsync(
                        DateTime.UtcNow.AddYears(-1),
                        email: HttpContext.User.GetCurrentUserDetails().Email);

                serviceRequests = results.ToList();
            }

            return View(new DashboardViewModel()
            {
                ServiceRequests = serviceRequests.OrderByDescending(p => p.RequestedDate).ToList(),
                ActiveServiceRequests = activeServiceRequests
            });
        }


    }
}