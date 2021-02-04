using Microsoft.AspNetCore.Mvc;
using ASC.Web.Controllers;
using ASC.Business.Interfaces;
using ASC.Models.Models;
using ASC.Utilities;
using System;
using System.Threading.Tasks;
using ASC.Web.Models.ServiceRequestViewModels;
using AutoMapper;
using ASC.Web.Data;
using System.Linq;
using ASC.Models.BaseTypes;
using ASC.Web.Controllers.Base;
using ASC.Web.Data.Cache;
using Microsoft.AspNetCore.Identity;
using ASC.Web.Models;
using System.Collections.Generic;

namespace ASC.Web.Controllers
{
    public class ServiceRequestController : BaseController
    {
        private readonly IServiceRequestOperations _serviceRequestOperations;
        private readonly IMapper _mapper;
        private readonly IMasterDataCacheOperations _masterData;
        private readonly UserManager<ApplicationUser> _userManager;

        public ServiceRequestController(IServiceRequestOperations serviceRequestOperations,
            IMapper mapper, IMasterDataCacheOperations masterData, UserManager<ApplicationUser> userManager)
        {
            _serviceRequestOperations = serviceRequestOperations;
            _mapper = mapper;
            _masterData = masterData;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> ServiceRequest()
        {
            MasterDataCache masterData = await _masterData.GetMasterDataCacheAsync();
            ViewBag.VehicleTypes = masterData.Values
                .Where(p => p.PartitionKey == nameof(MasterKeys.VehicleType))
                .ToList();
            ViewBag.VehicleNames = masterData.Values
                .Where(p => p.PartitionKey == nameof(MasterKeys.VehicleName))
                .ToList();

            return View(new NewServiceRequestViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ServiceRequest(NewServiceRequestViewModel request)
        {
            if (ModelState.IsValid == false)
            {
                var masterData = await _masterData.GetMasterDataCacheAsync();
                ViewBag.VehicleTypes = masterData.Values.Where(p => p.PartitionKey ==
                  nameof(MasterKeys.VehicleType)).ToList();
                ViewBag.VehicleNames = masterData.Values.Where(p => p.PartitionKey ==
                  nameof(MasterKeys.VehicleName)).ToList();
                return View(request);
            }

            var serviceRequest = _mapper.Map<ServiceRequest>(request);

            serviceRequest.PartitionKey = User.GetCurrentUserDetails().Email;
            serviceRequest.RowKey = Guid.NewGuid().ToString();
            serviceRequest.RequestedDate = request.RequestedDate;
            serviceRequest.Status = nameof(Status.New);
            serviceRequest.ServiceEngineer = HttpContext.User.IsInRole(nameof(Roles.Engineer))
                                             ? User.GetCurrentUserDetails().Email
                                             : string.Empty;

            await _serviceRequestOperations.CreateServiceRequestAsync(serviceRequest);

            return RedirectToAction("Dashboard", "Dashboard");

        }

        [HttpGet]
        public async Task<IActionResult> ServiceRequestDetails(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Error", "Home", new { id = "404" });
            }

            ServiceRequest serviceRequestDetails = await _serviceRequestOperations
                .GetServiceRequestByRowKey(rowKey: id);

            if (HttpContext.User.IsInRole(Roles.Engineer.ToString()) &&
                (serviceRequestDetails.ServiceEngineer != HttpContext.User.GetCurrentUserDetails().Email))
            {
                throw new UnauthorizedAccessException();
            }

            if (HttpContext.User.IsInRole(Roles.User.ToString()) &&
                serviceRequestDetails.PartitionKey != HttpContext.User.GetCurrentUserDetails().Email)
            {
                throw new UnauthorizedAccessException();
            }

            var serviceRequestAuditDetails = await _serviceRequestOperations
                .GetServiceRequestAuditByPartitionKey(serviceRequestDetails.PartitionKey + "-" + id);

            var masterData = await _masterData.GetMasterDataCacheAsync();
            ViewBag.VehicleTypes = masterData.Values.Where(p => p.PartitionKey == MasterKeys.VehicleType.ToString()).ToList();
            ViewBag.VehicleNames = masterData.Values.Where(p => p.PartitionKey == MasterKeys.VehicleName.ToString()).ToList();
            ViewBag.Status = Enum.GetValues(typeof(Status)).Cast<Status>().Select(v => v.ToString()).ToList();
            ViewBag.ServiceEngineers = await _userManager.GetUsersInRoleAsync(Roles.Engineer.ToString());



            return View(new ServiceRequestDetailViewModel
            {
                ServiceRequest = _mapper.Map<UpdateServiceRequestViewModel>(serviceRequestDetails),
                ServiceRequestAudit = serviceRequestAuditDetails.OrderByDescending(p => p.RequestedDate).ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateServiceRequestDetails(UpdateServiceRequestViewModel serviceRequest)
        {
            var originalServiceRequest = await _serviceRequestOperations.GetServiceRequestByRowKey(serviceRequest.RowKey);
            // Descrição do serviço
            originalServiceRequest.RequestedServices = serviceRequest.RequestedServices;

            // Update Status only if user role is either Admin or Engineer
            // Or Customer can update the status if it is only in Pending Customer Approval.
            if ((HttpContext.User.IsInRole(Roles.Admin.ToString()) ||
                HttpContext.User.IsInRole(Roles.Engineer.ToString())) ||
                (HttpContext.User.IsInRole(Roles.User.ToString()) &&
                originalServiceRequest.Status == Status.PendingCustomerApproval.ToString()))
            {
                originalServiceRequest.Status = serviceRequest.Status;
            }

            // Update Service Engineer field only if user role is Admin
            if (HttpContext.User.IsInRole(Roles.Admin.ToString()))
            {
                originalServiceRequest.ServiceEngineer = serviceRequest.ServiceEngineer;
            }

            await _serviceRequestOperations.UpdateServiceRequestAsync(originalServiceRequest);

            return RedirectToAction("ServiceRequestDetails", "ServiceRequest",
                new { id = serviceRequest.RowKey });

        }

        public async Task<IActionResult> CheckDenialService(DateTime requestedDate)
        {
            var serviceRequests = await _serviceRequestOperations
                .GetServiceRequestsByRequestedDateAndStatusAsync(
                  DateTime.UtcNow.AddDays(-90),
                  status: new List<string>() { Status.Denied.ToString() },
                  User.GetCurrentUserDetails().Email);

            if (serviceRequests.Any())
            {
                return Json(data: "There is a denied service request for you in last 90 days. Please contact ASC Admin.");
            }

            return Json(data: true);
        }

    }
}
