using ASC.Models.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASC.DataAccess.Interfaces
{
    public interface IServiceRequestRepository : IRepository<ServiceRequest>
    {
        Task<IEnumerable<ServiceRequest>> GetDashboardQueryAsync(DateTime? requestedDate, List<string> status = null, string email = "", string serviceEngineerEmail = "");
        Task<List<ServiceRequest>> GetStatusServiceRequestsAsync(List<string> status);
    }
}
