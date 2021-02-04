using System;
using System.Collections.Generic;
using System.Text;
using ASC.Models.Models;
using System.Threading.Tasks;

namespace ASC.Business.Interfaces
{
    /// <summary>
    /// This interface will hold all the key methods which are used to insert and
    /// update a service request details in MongoDB Table storage.
    /// </summary>
    public interface IServiceRequestOperations
    {
        Task CreateServiceRequestAsync(ServiceRequest request);
        Task<ServiceRequest> UpdateServiceRequestAsync(ServiceRequest request);
        Task<ServiceRequest> UpdateServiceRequestStatusAsync(string rowKey, string partitionKey, string status);
        Task<IEnumerable<ServiceRequest>> GetServiceRequestsByRequestedDateAndStatusAsync(DateTime? requestedDate, List<string> status = null, string email = "", string serviceEngineerEmail = "");
        Task<List<ServiceRequest>> GetActiveServiceRequestsAsync(List<string> status);
        Task<ServiceRequest> GetServiceRequestByRowKey(string rowKey);
        Task<List<ServiceRequest>> GetServiceRequestAuditByPartitionKey(string partitionKey);
    }
}
