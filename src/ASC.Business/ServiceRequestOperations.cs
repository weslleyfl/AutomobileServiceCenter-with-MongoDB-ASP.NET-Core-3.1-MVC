using System;
using System.Collections.Generic;
using System.Text;
using ASC.Business.Interfaces;
using ASC.DataAccess.Interfaces;
using ASC.Models.Models;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace ASC.Business
{
    public class ServiceRequestOperations : IServiceRequestOperations
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceRequestRepository _serviceRequestRepository;

        public ServiceRequestOperations(IUnitOfWork unitOfWork, IServiceRequestRepository serviceRequestRepository)
        {
            _unitOfWork = unitOfWork;
            _serviceRequestRepository = serviceRequestRepository;
        }

        public async Task<IEnumerable<ServiceRequest>> GetServiceRequestsByRequestedDateAndStatusAsync(DateTime? requestedDate, List<string> status = null, string email = "", string serviceEngineerEmail = "")
        {
            IEnumerable<ServiceRequest> listServices = await _serviceRequestRepository
                .GetDashboardQueryAsync(requestedDate, status, email, serviceEngineerEmail);
            return listServices;
        }

        public async Task CreateServiceRequestAsync(ServiceRequest request)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Repository<ServiceRequest>().Add(request);
                await _unitOfWork.CommitTransactionAsync();
            }
        }

        public async Task<ServiceRequest> UpdateServiceRequestAsync(ServiceRequest request)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Repository<ServiceRequest>().Update(request);
                await _unitOfWork.CommitTransactionAsync();

                return request;
            }
        }

        public async Task<ServiceRequest> UpdateServiceRequestStatusAsync(string rowKey, string partitionKey, string status)
        {
            using (_unitOfWork)
            {
                ServiceRequest serviceRequest = await _unitOfWork.Repository<ServiceRequest>()
                    .FindAsync(rowKey);

                if (serviceRequest == null)
                {
                    throw new ArgumentNullException();
                }

                serviceRequest.Status = status;

                _unitOfWork.Repository<ServiceRequest>().Update(serviceRequest);
                await _unitOfWork.CommitTransactionAsync();

                return serviceRequest;
            }
        }

        public async Task<List<ServiceRequest>> GetActiveServiceRequestsAsync(List<string> status)
        {
            List<ServiceRequest> listServices = await _serviceRequestRepository.GetStatusServiceRequestsAsync(status);
            return listServices;
        }

        public async Task<ServiceRequest> GetServiceRequestByRowKey(string rowKey)
        {
            return await _unitOfWork.Repository<ServiceRequest>().FindAsync(rowKey);
        }

        public async Task<List<ServiceRequest>> GetServiceRequestAuditByPartitionKey(string partitionKey)
        {
            var serviceRequests = await _unitOfWork.Repository<ServiceRequest>().FindAllByPartitionKeyAsync(partitionKey);
            return serviceRequests.ToList();
        }
    }
}
